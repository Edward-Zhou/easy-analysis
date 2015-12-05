using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.Data;
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

            IReadOnlyCollection datasource = MongoDataCollection.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            var collection = datasource.GetData() as IMongoCollection<BsonDocument>;

            var filterBuilder = Builders<BsonDocument>.Filter;

            var filter = filterBuilder.Type("createdOn", BsonType.String);

            if (timeFrameRange != null)
            {
                filter = filter & MongoHelper.CreateTimeFrameFilter(timeFrameRange);
            }

            await collection
                    .Find(filter)
                    .ForEachAsync(async (item) => {
                        var id = item.GetValue("_id").AsString;

                        var dateText = item.GetValue("createdOn").AsString;

                        var idFilter = Builders<BsonDocument>.Filter.Eq("_id", id);

                        var update = Builders<BsonDocument>.Update.Set("createdOn", DateTime.Parse(dateText));

                        await collection.UpdateOneAsync(idFilter, update);
                    });
        }
    }
}
