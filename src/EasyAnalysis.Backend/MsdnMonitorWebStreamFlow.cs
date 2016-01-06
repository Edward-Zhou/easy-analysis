using EasyAnalysis.Framework;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.IO;
using EasyAnalysis.Modules;
using System;
using System.Collections.Generic;

namespace EasyAnalysis.Backend
{
    public class MsdnMonitorWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string settingName = parameters[0].ToLower();

            string cacheFolder = parameters[1];

            string outputCollectionName = parameters[2];

            var generalDataFlowConfigration = new GeneralStreamFlowConfigration
            {
                ProcessModules = new List<string>
                {
                    "msdn-metadata-module"
                },
                UseCache = false
            };

            var jsonConfigrationManager = new JsonConfigrationManager<PaginationDiscoveryConfigration>("config.json");

            var paginationDiscoveryConfigration = jsonConfigrationManager.GetSetting(settingName);

            if (paginationDiscoveryConfigration == null)
            {
                throw new ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            IResourceDiscovery discovery = new PeriodicPaginationDiscovery(paginationDiscoveryConfigration);

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
