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
            var temp = args[0].Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            var mongoClient = new MongoClient(_mongoconnectionStringProvider.GetConnectionString(databaseName));

            var database = mongoClient.GetDatabase(databaseName);

            var collection = database.GetCollection<BsonDocument>(collectionName);

            await collection
                    .Find("{ createdOn: { $type: 2 } }")
                    .ForEachAsync((item) => {
                        var id = item.GetValue("_id").AsString;

                        var dateText = item.GetValue("createdOn").AsString;

                        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

                        var update = Builders<BsonDocument>.Update.Set("createdOn", DateTime.Parse(dateText));

                        collection.UpdateOneAsync(filter, update);
                    });
        }
    }
}
