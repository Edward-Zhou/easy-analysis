using EasyAnalysis.Data;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class BasicDocumentAction
    {
        private IConnectionStringProvider _connectionStringProvider = new UniversalConnectionStringProvider();

        protected static void SetValuesToCollection(IDictionary<string, object> values, IMongoCollection<BsonDocument> output, bool isUpsert = false)
        {
            var identifier = Builders<BsonDocument>.Filter.Eq("_id", values["_id"]);

            values.Remove("_id");

            var updateBuilder = Builders<BsonDocument>.Update;

            UpdateDefinition<BsonDocument> updateDefination = null;

            foreach (var kv in values)
            {
                if (updateDefination == null)
                {
                    updateDefination = updateBuilder.Set(kv.Key, kv.Value);
                }
                else
                {
                    updateDefination = updateDefination.Set(kv.Key, kv.Value);
                }
            }

            var task = output.UpdateOneAsync(identifier, updateDefination, new UpdateOptions {  IsUpsert = isUpsert });

            task.Wait();
        }

        protected IMongoCollection<BsonDocument> CreateCollection(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + databaseName));

            var database = client.GetDatabase(databaseName);

            var collection = database.GetCollection<BsonDocument>(collectionName);

            return collection;
        }
    }
}
