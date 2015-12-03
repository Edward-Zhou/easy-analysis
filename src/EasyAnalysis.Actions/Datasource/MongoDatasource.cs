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
    public class MongoDatasource
    {
        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        private IConnectionStringProvider _connectionStringProvider;

        public MongoDatasource()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }

        public static MongoDatasource Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            return new MongoDatasource
            {
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
        }

        public IMongoCollection<BsonDocument> GetCollection()
        {
            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + DatabaseName));

            var database = client.GetDatabase(DatabaseName);

            var collection = database.GetCollection<BsonDocument>(CollectionName);

            return collection;
        }
    }
}
