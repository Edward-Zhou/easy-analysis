using Dapper;
using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class SyncWithStackoverflowTags : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Sync up tags for stackoverflow cases with external system";
            }
        }

        public SyncWithStackoverflowTags(IConnectionStringProvider connectionStringProvider = null)
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
        /// 
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
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

            try
            {
                dynamic soThreads = null;

                using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("SoDBConnection")))
                {
                    var inputDs = new NamedQueryReadOnlyCollection("import_thread_tags_" + repository, connection);

                    inputDs.SetParameters(new
                    {
                        start = timeFrameRange.Start,
                        end = timeFrameRange.End
                    });

                    soThreads = inputDs.GetData();
                }
                
                var host = "analyzeit.azurewebsites.net";
                //var host = "localhost:58277"; //For Debugging

                using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
                {
                    var tpDs = new NamedQueryReadOnlyCollection("query_thread_existence", connection);

                    foreach (dynamic row in soThreads)
                    {
                        tpDs.SetParameters(new
                        {
                            id = "SO_" + row.question_id
                        });

                        dynamic q = (tpDs.GetData() as IEnumerable<dynamic>).First();

                        int recordExist = q.Total;

                        if (recordExist > 0)
                        {
                            String[] tags = row.tags.ToString().Split(';');

                            foreach (string tag in tags)
                            {
                                string apiUrl = "http://{0}/api/thread/{1}/tag/";
                                using (var client = new HttpClient())
                                {
                                    var request = new StringContent("=" + tag)
                                    {
                                        Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                                    };
                                    var response = await client.PostAsync(string.Format(apiUrl, host, "SO_" + row.question_id), request);
                                    //response.EnsureSuccessStatusCode();
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }
    }
}
