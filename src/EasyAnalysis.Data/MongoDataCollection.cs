using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace EasyAnalysis.Data
{
    public class MongoDataCollection : IReadOnlyCollection
    {
        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        private IConnectionStringProvider _connectionStringProvider;

        public MongoDataCollection()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }

        public static MongoDataCollection Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            return new MongoDataCollection
            {
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
        }

        public Object GetData()
        {
            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + DatabaseName));

            var database = client.GetDatabase(DatabaseName);

            var collection = database.GetCollection<BsonDocument>(CollectionName);

            return collection;
        }
    }
}
