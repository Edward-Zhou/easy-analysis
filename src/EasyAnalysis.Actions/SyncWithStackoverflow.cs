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
        /// [0-timeframe      (optional), e.g. 2015-10-01T00:00:00&2015-11-30T00:00:00]
        /// </param>
        /// <returns></returns>
        public Task RunAsync(string[] args)
        {
            TimeFrameRange timeFrameRange = null;

            if (args.Length > 0)
            {
                timeFrameRange = TimeFrameRange.Parse(args[0]);
            }

            var task = new Task(() =>
            {
                try
                {
                    // query the exteranl database
                    DataTable soTable = new DataTable();

                    // Creates a SQL connection
                    using (var conn = new SqlConnection(_connectionStringProvider.GetConnectionString("SoDBConnection")))
                    {
                        conn.Open();

                        if (timeFrameRange != null)
                        {
                            // Creates a SQL command
                            using (var command = new SqlCommand("SELECT * FROM [vSOUWPAnalysis] WHERE [CreatedOn] >= @start AND [CreatedOn] <= @end", conn))
                            {
                                command.Parameters.AddWithValue("start", timeFrameRange.Start);
                                command.Parameters.AddWithValue("end", timeFrameRange.End);
                                // Loads the query results into the table
                                soTable.Load(command.ExecuteReader());
                            }
                        }
                        else
                        {
                            // Creates a SQL command
                            using (var command = new SqlCommand("SELECT * FROM [vSOUWPAnalysis]", conn))
                            {
                                // Loads the query results into the table
                                soTable.Load(command.ExecuteReader());
                            }
                        }
                        
                        conn.Close();
                    }

                    // IF NOT EXISTS
                    // import the data to EAS web database
                    // ELSE SKIP
                    using (var conn = new SqlConnection(_connectionStringProvider.GetConnectionString()))
                    {
                        foreach (DataRow dr in soTable.Rows)
                        {
                            dynamic q = conn.Query("SELECT COUNT(*) AS [Total] FROM [Threads] WHERE ([Id] = @id)", new { id = "SO_" + dr[0] }).First();

                            int recordExist = q.Total;

                            if (recordExist == 0)
                            {
                                conn.Execute("INSERT [Threads] ([Id], [Title], [CreateOn], [ForumId], [AuthorId]) VALUES (@id, @title, @createOn, @forumId, @authorId)", new { id = "SO_" + dr[0], title = dr[1], createOn = dr[3], forumId = "stackoverflow.uwp", authorId = "SO_" + dr[4] });
                            }
                        }

                    }

                    // ID FORMAT: SO_{QUESTION_ID}

                    // FORUM_ID: stackoverflow.uwp
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
