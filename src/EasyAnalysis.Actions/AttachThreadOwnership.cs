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
                SetValuesToCollection(values, output);
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
