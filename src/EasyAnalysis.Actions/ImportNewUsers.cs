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
            var ds = MongoDatasource.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if(args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + ds.DatabaseName));

            IMongoDatabase database = client.GetDatabase(ds.DatabaseName);

            IMongoCollection<BsonDocument> sourceCollection = database.GetCollection<BsonDocument>(ds.CollectionName);

            FilterDefinition<BsonDocument> filter = "{}";

            var filterBuilder = Builders<BsonDocument>.Filter;

            if (timeFrameRange != null)
            {
                filter = filter &
                         filterBuilder.Gte("timestamp", timeFrameRange.Start) &
                         filterBuilder.Lte("timestamp", timeFrameRange.End);
            }

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString("EasIndexConnection")))
            {
                await sourceCollection
                      .Find(filter)
                      .ForEachAsync((thread) =>
                      {
                          foreach (BsonDocument user in thread.GetElement("users").Value.AsBsonArray)
                          {
                              Logger.Current.Info(user.GetValue("display_name").AsString);

                              var id = user.GetValue("id").AsString;

                              var displayName = user.GetValue("display_name").AsString;

                              var msft = bool.Parse(user.GetValue("msft").AsString);

                              var mscs = bool.Parse(user.GetValue("mscs").AsString);

                              var mvp =  bool.Parse(user.GetValue("mvp").AsString);

                              var partner = bool.Parse(user.GetValue("partner").AsString);

                              var mcc = bool.Parse(user.GetValue("mcc").AsString);

                              var match = connection.Query(SqlQueryFactory.Instance.Get("find_user_by_id"), new { Id = id });

                              if(match.Count() == 0)
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
                              }
                          }
                      });
            }
        }
    }
}
