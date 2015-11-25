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
        /// [0-timeframe      (optional), e.g. 2015-10-01T00:00:00&2015-11-30T00:00:00]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            TimeFrameRange timeFrameRange = null;

            if (args != null && args.Length > 0)
            {
                timeFrameRange = TimeFrameRange.Parse(args[0]);
            }

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
                        using (var command = new SqlCommand("SELECT [question_id], [tags] FROM [vSOUWPAnalysis] v LEFT JOIN [Question] q ON v.Id = q.question_id WHERE [CreatedOn] >= @start AND [CreatedOn] < DateAdd(Day, 1, @end)", conn))
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
                        using (var command = new SqlCommand("SELECT [question_id], [tags] FROM [vSOUWPAnalysis] v LEFT JOIN [Question] q ON v.Id = q.question_id", conn))
                        {
                            // Loads the query results into the table
                            soTable.Load(command.ExecuteReader());
                        }
                    }

                    conn.Close();
                }


                var host = "analyzeit.azurewebsites.net";
                //var host = "localhost:58277"; //For Debug

                // IF NOT EXISTS
                // import Tags into EAS web database
                // ELSE SKIP
                using (var conn = new SqlConnection(_connectionStringProvider.GetConnectionString()))
                {
                    foreach (DataRow dr in soTable.Rows)
                    {
                        dynamic q = conn.Query("SELECT COUNT(*) AS [Total] FROM [Threads] WHERE ([Id] = @id)", new { id = "SO_" + dr[0] }).First();

                        int recordExist = q.Total;

                        if (recordExist > 0)
                        {
                            String[] tags = dr[1].ToString().Split(';');
                            foreach (string tag in tags)
                            {
                                string apiUrl = "http://{0}/api/thread/{1}/tag/";
                                using (var client = new HttpClient())
                                {
                                    var request = new StringContent("=" + tag)
                                    {
                                        Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                                    };
                                    var response = await client.PostAsync(string.Format(apiUrl, host, "SO_" + dr[0]), request);
                                    response.EnsureSuccessStatusCode();
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
