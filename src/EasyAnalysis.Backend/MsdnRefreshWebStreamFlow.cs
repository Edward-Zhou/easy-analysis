using EasyAnalysis.Actions;
using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.IO;
using EasyAnalysis.Modules;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    public class MsdnRefreshWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string cacheFolder = parameters[0];

            string inputCollectionName = parameters[1];

            string outputCollectionName = parameters[2];

            var dateRange = TimeFrameRange.Parse(parameters[3]);

            var generalDataFlowConfigration = new GeneralStreamFlowConfigration
            {
                ProcessModules = new List<string>
                {
                    "msdn-metadata-module"
                },
                UseCache = false
            };

            var collection = Data.MongoDataCollection.Parse(inputCollectionName);

            var data = collection.GetData() as IMongoCollection<BsonDocument>;

            var fb = Builders<BsonDocument>.Filter;

            var filter = fb.Gt("createdOn", dateRange.Start) & fb.Lt("createdOn", dateRange.End) & fb.Exists("del", false);

            var list = new List<string>();

            var task = data.Find(filter)
                .Project("{url: 1}")
                .ForEachAsync((item) =>
                {
                    list.Add(item.GetValue("url", BsonValue.Create("")).AsString + "&outputAs=xml");
                });

            task.Wait();

            IResourceDiscovery discovery = new ListDiscovery(list);

            var moduleFactory = new DefaultModuleFactory();

            var cacheService = new LocalFileCacheServcie();

            cacheService.Configure(cacheFolder);

            var output = new MongoCollectionOutput(outputCollectionName);

            var dataflow = new GeneralStreamFlow(
                config: generalDataFlowConfigration,
                uriDiscovery: discovery,
                moduleFactory: moduleFactory,
                cacheServcie: cacheService,
                output: output);

            dataflow.Init();

            dataflow.Run();
        }
    }
}
