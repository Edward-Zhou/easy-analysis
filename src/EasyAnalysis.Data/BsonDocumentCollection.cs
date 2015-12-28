using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyAnalysis.Data
{
    public class BsonDocumentCollection : Framework.Data.IReadOnlyCollection<BsonDocument>
    {
        private string _databaseName;

        private string _collectionName;

        private IMongoCollection<BsonDocument> _collection;

        private FilterDefinition<BsonDocument> _filter;

        public string DatabaseName { get {
                return _databaseName;
            }
        }

        public string CollectionName { get {
                return _collectionName;
            }
        }

        public IMongoCollection<BsonDocument> Raw
        {
            get
            {
                return InternalGetCollection();
            }
        }

        private IConnectionStringProvider _connectionStringProvider;

        public BsonDocumentCollection()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }

        public BsonDocumentCollection(IMongoCollection<BsonDocument> collection, FilterDefinition<BsonDocument> filter)
        {
            _collection = collection;

            _filter = filter;
        }

        public BsonDocumentCollection(string source)
        {
            var temp = source.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            _filter = "{}";
        }

        public static BsonDocumentCollection Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            return new BsonDocumentCollection
            {
                _databaseName = databaseName,
                _collectionName = collectionName
            };
        }

        public void Filter(
            Func<FilterDefinitionBuilder<BsonDocument>, FilterDefinition<BsonDocument>> filterBuilderFunc)
        {
            _filter = filterBuilderFunc(Builders<BsonDocument>.Filter);
        }

        public async Task ForEachAsync(Action<BsonDocument> processor)
        {
            var collection = InternalGetCollection();

            await collection.Find(_filter)
                            .ForEachAsync(processor);
        }

        public async Task ForEachAsync(Func<BsonDocument, Task> processor)
        {
            var collection = InternalGetCollection();

            await collection.Find(_filter)
                      .ForEachAsync(processor);
        }

        private IMongoCollection<BsonDocument>  InternalGetCollection()
        {
            if(_collection != null)
            {
                return _collection;
            }

            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + DatabaseName));

            var database = client.GetDatabase(DatabaseName);

            _collection = database.GetCollection<BsonDocument>(CollectionName);

            return _collection;
        }
    }
}
