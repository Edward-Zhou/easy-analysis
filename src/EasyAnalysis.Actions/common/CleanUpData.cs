using EasyAnalysis.Data;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource-in  (required), e.g. landing.threads]
        /// [1-timeframe      (optional), e.g. 2015-11-16T00:00:00&2015-11-18T00:00:00]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            if(args == null || args.Length == 0)
            {
                throw new ArgumentException("Invalid action auguments for clean up data");
            }

            BsonDocumentCollection collection = new BsonDocumentCollection(args[0]);

            collection.Filter((fb) => {
                var filter = fb.Type("createdOn", BsonType.String);

                if (args.Length > 1)
                {
                    filter = filter & MongoHelper.CreateTimeFrameFilter(TimeFrameRange.Parse(args[1]));
                }

                return filter;
            });

            var pipeline = new DataProcessingPipeLine<BsonDocument>
            {
                Source = collection
            };

            pipeline.OnOutput += async (outputRecord) => {
                var idFilter = Builders<BsonDocument>.Filter.Eq("_id", outputRecord["_id"]);

                var update = Builders<BsonDocument>.Update.Set("createdOn", outputRecord["createdOn"]);

                await collection.Raw.UpdateOneAsync(idFilter, update);
            };

            await pipeline.Process((context, item) =>
            {
                var id = item.GetValue("_id").AsString;

                var dateText = item.GetValue("createdOn").AsString;

                context.Output(new Dictionary<string, object> {
                    { "_id", id},
                    { "createdOn", DateTime.Parse(dateText)}
                });
            });
        }
    }
}
