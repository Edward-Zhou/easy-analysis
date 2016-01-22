using EasyAnalysis.Data;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace EasyAnalysis.Backend
{
    public class ImportStackOverflowDataFlow : IWebStreamFlow
    {
        public class StackOverflowImportModel
        {
            public string Id { get; set; }

            public string title { get; set; }

            public string link { get; set; }

            public DateTime CreatedOn { get; set; }

            public string AuthorId { get; set; }
        }

        private IConnectionStringProvider _connectionStringProvider;

        public ImportStackOverflowDataFlow()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }

        /// <summary>
        /// named query name
        /// target collectoin name
        /// extra parameters
        /// </summary>
        /// <param name="parameters"></param>
        public void Run(string[] parameters)
        {
            var namedQueryName = parameters[0];

            var targetCollectionName = parameters[1];

            var output = CreateCollection(targetCollectionName);

            var collection = new SqlNamedQueryCollection<StackOverflowImportModel>("SoDBConnection", namedQueryName);

            collection.SetParameters(new { start = DateTime.Parse("2016-1-1"), end = DateTime.Parse("2016-1-20") });

            var pipeline = new DataProcessingPipeLine<StackOverflowImportModel>
            {
                Source = collection
            };

            pipeline.OnOutput += (values) =>
            {
                SetValuesToCollection(values: values, output: output, isUpsert: true);
            };

            var task = pipeline.Process((context, item) =>
            {
                var values = new Dictionary<string, object>
                {
                    { "_id", item.Id},
                    { "title", item.title },
                    { "url", item.link }
                };

                context.Output(values);
            });

            task.Wait();
        }

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

            var task = output.UpdateOneAsync(identifier, updateDefination, new UpdateOptions { IsUpsert = isUpsert });

            task.Wait();
        }

        /// <summary>
        /// CODE REFACTOR
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
