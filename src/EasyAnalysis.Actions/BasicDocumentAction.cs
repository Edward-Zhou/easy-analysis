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
