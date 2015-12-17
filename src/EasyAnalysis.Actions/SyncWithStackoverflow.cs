using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EasyAnalysis.Framework;
using EasyAnalysis.Data;

namespace EasyAnalysis.Actions
{
    public class SyncWithStackoverflow : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Sync up stackoverflow data with external system";
            }
        }

        public SyncWithStackoverflow(IConnectionStringProvider connectionStringProvider = null)
        {
            if (connectionStringProvider == null)
            {
                _connectionStringProvider = new UniversalConnectionStringProvider();
            }
            else
            {
                _connectionStringProvider = connectionStringProvider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-repository (required), e.g. SOUWP]
        /// [1-timeframe(required), e.g. 2015-10-01T00:00:00Z&2015-11-30T00:00:00Z]
        /// </param>
        /// <returns></returns>
        public Task RunAsync(string[] args)
        {
            var repository = args[0].ToLower();
            if (repository == null || !repository.StartsWith("so"))
            {
                throw new Exception("The first parameter: repository, e.g. SOUWP, should start with so prefix");
            }

            var timeFrameRange = TimeFrameRange.Parse(args[1]);

            if (timeFrameRange == null)
            {
                throw new Exception("The second parameter: timeframe, e.g. 2015-10-01T00:00:00Z&2015-11-30T00:00:00Z");
            }

            var task = new Task(() =>
            {
                try
                {
                    dynamic soThreads = null;

                    using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("SoDBConnection")))
                    {
                        var inputDs = new NamedQueryReadOnlyCollection("import_thread_" + repository, connection);

                        inputDs.SetParameters(new
                        {
                            start = timeFrameRange.Start,
                            end = timeFrameRange.End
                        });

                        soThreads = inputDs.GetData();
                    }

                    using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
                    {
                        var tpDs = new NamedQueryReadOnlyCollection("query_thread_existence", connection);

                        var outputDs = new NamedQueryReadOnlyCollection("insert_new_thread", connection);

                        foreach (dynamic row in soThreads)
                        {
                            tpDs.SetParameters(new
                            {
                                id = "SO_" + row.Id
                            });

                            dynamic q = (tpDs.GetData() as IEnumerable<dynamic>).First();

                            int recordExist = q.Total;

                            if (recordExist == 0)
                            {
                                string tp_repo = repository.Substring(2); //remove "so" prefix

                                outputDs.SetParameters(new
                                {
                                    id = "SO_" + row.Id,
                                    title = row.title,
                                    createOn = row.CreatedOn,
                                    forumId = "stackoverflow." + tp_repo,
                                    authorId = "SO_" + row.AuthorId
                                });

                                outputDs.GetData();
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.Current.Error(ex.Message);
                }
            });

            task.Start();
            return task;
        }
    }
}
