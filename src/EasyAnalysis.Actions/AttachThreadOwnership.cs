using EasyAnalysis.Data;
using EasyAnalysis.Framework.Analysis;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class AttachThreadOwnership : BasicDocumentAction, IAction
    {
        public string Description
        {
            get
            {
                return "Attach the thread ownership mongo collection";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [intput: file path]
        /// [output: mongo collection]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var collection = new HtmlTableCollection(args[0], "//*[@id='gridBodyTable']/tr");

            var output = CreateCollection(args[1]);

            var pipeline = new DataProcessingPipeLine<BsonDocument>
            {
                Source = collection
            };

            pipeline.OnOutput += (values) =>
            {
                var identifier = Builders<BsonDocument>.Filter.Eq("_id", values["_id"]);

                values.Remove("_id");

                var updateBuilder = Builders<BsonDocument>.Update;

                UpdateDefinition<BsonDocument> updateDefination = null;

                foreach (var kv in values)
                {
                    if (updateDefination == null)
                    {
                        updateDefination = updateBuilder.Set(kv.Key, kv.Value as string);
                    }
                    else
                    {
                        updateDefination = updateDefination.Set(kv.Key, kv.Value as string);
                    }
                }

                var task = output.UpdateOneAsync(identifier, updateDefination);

                task.Wait();
            };

            await pipeline.Process((context, item) =>
            {
                var values = new Dictionary<string, object>
                {
                    { "_id", item.GetValue("External ID (Thread)").AsString},
                    { "owner", item.GetValue("Owner").AsString }
                };

                context.Output(values);
            });
        }
    }
}
