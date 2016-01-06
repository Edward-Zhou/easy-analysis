using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace EasyAnalysis.Infrastructure.IO
{
    public class MongoCollectionOutput : IOutput
    {
        private IMongoCollection<BsonDocument> _outputCollection;

        public MongoCollectionOutput(string name)
        {
            var temp = name.Split('.');

            var dbName = temp[0];

            var collectionName = temp[1];

            var mongoconnectionStringProvider = new MongoDBConnectionStringProvider();

            var connectionString = mongoconnectionStringProvider.GetConnectionString(dbName);

            var mongoClient = new MongoClient(connectionString);

            var database = mongoClient.GetDatabase(dbName);

            _outputCollection = database.GetCollection<BsonDocument>(collectionName);
        }

        public void Output(IDictionary<string, object> data)
        {
            var id = Builders<BsonDocument>.Filter.Eq("_id", data["_id"] as string);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            using (var jsonReader = new JsonReader(jsonString))
            {
                var context = BsonDeserializationContext.CreateRoot(jsonReader);

                var document = _outputCollection.DocumentSerializer.Deserialize(context);

                document.Set("timestamp", DateTime.Now);

                _outputCollection.DeleteOneAsync(id).Wait();

                _outputCollection.InsertOneAsync(document).Wait();
            }
        }
    }
}
