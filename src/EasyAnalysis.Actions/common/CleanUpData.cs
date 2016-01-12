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
    public class CleanUpData : BasicDocumentAction, IAction
    {
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
            FilterDefinition<BsonDocument> filter = ParseFilterFromArgs(args);

            var process = new DocumentToDocumentProcess
            {
                In = CreateCollection(args[0]),
                Filter = filter,
                Out = CreateCollection(args[0])
            };

            await process.ProcessAsync((list, document) =>
            {
                var id = document.GetValue("_id").AsString;

                var dateText = document.GetValue("createdOn").AsString;

                list.Add(new Dictionary<string, object> {
                    { "_id", id},
                    { "createdOn", DateTime.Parse(dateText)}
                });
            });
        }

        private static FilterDefinition<BsonDocument> ParseFilterFromArgs(string[] args)
        {
            var filter = Builders<BsonDocument>.Filter.Type("createdOn", BsonType.String);

            if (args.Length > 1)
            {
                filter = filter & MongoFilterHelper.CreateTimeFrameFilter(TimeFrameRange.Parse(args[1]));
            }

            return filter;
        }
    }
}
