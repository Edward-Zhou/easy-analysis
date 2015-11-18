using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class CleanUpData : IAction
    {
        private IConnectionStringProvider _mongoconnectionStringProvider;

        public CleanUpData()
        {
            _mongoconnectionStringProvider = new MongoDBConnectionStringProvider();
        }

        public string Description
        {
            get
            {
                return "clean up data";
            }
        }

        public async Task RunAsync(string[] args)
        {
            if(args == null || args.Length == 0)
            {
                throw new ArgumentException("Invalid action auguments for clean up data");
            }

            var datasource = MongoDatasource.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            var mongoClient = new MongoClient(_mongoconnectionStringProvider.GetConnectionString(datasource.DatabaseName));

            var database = mongoClient.GetDatabase(datasource.DatabaseName);

            var collection = database.GetCollection<BsonDocument>(datasource.CollectionName);

            var filterBuilder = Builders<BsonDocument>.Filter;

            var filter = filterBuilder.Type("createdOn", BsonType.String);

            if (timeFrameRange != null)
            {
                filter = filter &
                         filterBuilder.Gte("timestamp", timeFrameRange.Start) &
                         filterBuilder.Lte("timestamp", timeFrameRange.End);
            }

            await collection
                    .Find(filter)
                    .ForEachAsync((item) => {
                        var id = item.GetValue("_id").AsString;

                        var dateText = item.GetValue("createdOn").AsString;

                        var idFilter = Builders<BsonDocument>.Filter.Eq("_id", id);

                        var update = Builders<BsonDocument>.Update.Set("createdOn", DateTime.Parse(dateText));

                        collection.UpdateOneAsync(idFilter, update);
                    });
        }
    }
}
