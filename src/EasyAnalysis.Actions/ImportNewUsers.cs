using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System;
using EasyAnalysis.Framework.Data;

namespace EasyAnalysis.Actions
{
    public class ImportNewUsers : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Import new users to current users collection";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringProvider"></param>
        public ImportNewUsers(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource (required), e.g. landing.threads] 
        /// [1-timeframe  (optional), e.g. 2015-11-16T00:00:00&2015-11-18T00:00:00]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            IReadOnlyCollection ds = MongoDataCollection.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            IMongoCollection<BsonDocument> sourceCollection = ds.GetData() as IMongoCollection<BsonDocument>;

            FilterDefinition<BsonDocument> filter = "{}";

            if(timeFrameRange != null)
            {
                filter = filter & MongoHelper.CreateTimeFrameFilter(timeFrameRange);
            }

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("EasIndexConnection")))
            {
                await sourceCollection
                      .Find(filter)
                      .ForEachAsync((thread) =>
                      {
                          foreach (BsonDocument user in thread.GetElement("users").Value.AsBsonArray)
                          {
                              ImportNewUser(connection, user);
                          }
                      });
            }
        }

        private static void ImportNewUser(SqlConnection connection, BsonDocument user)
        {
            try
            {
                var id = user.GetValue("id").AsString;

                var displayName = user.GetValue("display_name").AsString;

                var msft = bool.Parse(user.GetValue("msft").AsString);

                var mscs = bool.Parse(user.GetValue("mscs").AsString);

                var mvp = bool.Parse(user.GetValue("mvp").AsString);

                var partner = bool.Parse(user.GetValue("partner").AsString);

                var mcc = bool.Parse(user.GetValue("mcc").AsString);

                var match = connection.Query(SqlQueryFactory.Instance.Get("find_user_by_id"), new { Id = id });

                if (match.Count() == 0)
                {
                    connection.Execute(SqlQueryFactory.Instance.Get("insert_user"), new
                    {
                        Id = id,
                        DisplayName = displayName,
                        Msft = msft,
                        Mscs = mscs,
                        Mvp = mvp,
                        Partner = partner,
                        Mcc = mcc,
                        Timestamp = DateTime.Now
                    });

                    Logger.Current.Info("Add new user [" + displayName + "]");
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }
    }
}
