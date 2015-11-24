using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
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
    public class BuildBasicThreadProfiles : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Build the basic thread profiles";
            }
        }

        public BuildBasicThreadProfiles(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource-in  (required), e.g. landing.threads]
        /// [1-datasource-out (required), e.g. uwp.thread_profiles]
        /// [2-timeframe      (optional), e.g. 2015-11-16T00:00:00&2015-11-18T00:00:00]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            Logger.Current.Info(Description);

            var dsInput = MongoDatasource.Parse(args[0]);

            var dsOutput = MongoDatasource.Parse(args[1]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 2)
            {
                timeFrameRange = TimeFrameRange.Parse(args[2]);
            }

            IMongoCollection<BsonDocument> inputCollection = GetCollection(dsInput);

            IMongoCollection<BsonDocument> outputCollection = GetCollection(dsOutput);

            FilterDefinition<BsonDocument> filter = "{}";

            var filterBuilder = Builders<BsonDocument>.Filter;

            if (timeFrameRange != null)
            {
                filter = filter &
                         filterBuilder.Gte("timestamp", timeFrameRange.Start) &
                         filterBuilder.Lte("timestamp", timeFrameRange.End);
            }

            await inputCollection
                      .Find(filter)
                      .ForEachAsync(async (item) =>
                      {
                          await ParseAndOutput(item, outputCollection);
                      });
        }

        private static async Task ParseAndOutput(BsonDocument item, IMongoCollection<BsonDocument> outputCollection)
        {
            try
            {
                var html = item.GetValue("messages")
                  .AsBsonArray
                  .FirstOrDefault()
                  .AsBsonDocument
                  .GetValue("body").AsString;

                var document = new HtmlAgilityPack.HtmlDocument();

                document.LoadHtml(html);

                var text = document.DocumentNode.InnerText;

                var excerpt = text.Substring(0, Math.Min(256, text.Length));

                var updateAction = Builders<BsonDocument>.Update
                                    .Set("createdOn", item.GetValue("createdOn").ToUniversalTime())
                                    .Set("title", item.GetValue("title").AsString)
                                    .Set("url", item.GetValue("url").AsString)
                                    .Set("answered", bool.Parse(item.GetValue("answered").AsString))
                                    .Set("excerpt", excerpt);

                var identity = Builders<BsonDocument>.Filter.Eq("_id", item.GetValue("_id").AsString);

                await outputCollection.UpdateOneAsync(
                      identity,
                      updateAction,
                      new UpdateOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }

        private IMongoCollection<BsonDocument> GetCollection(MongoDatasource ds)
        {
            var client = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + ds.DatabaseName));

            var database = client.GetDatabase(ds.DatabaseName);

            var collection = database.GetCollection<BsonDocument>(ds.CollectionName);

            return collection;
        }
    }
}
