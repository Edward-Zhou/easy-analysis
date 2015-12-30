using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.IO;
using EasyAnalysis.Modules;
using System.Collections.Generic;
using System;

namespace EasyAnalysis.Backend
{
    public class MsdnInitWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string settingName = parameters[0].ToLower();

            string cacheFolder = parameters[1];

            string outputCollectionName = parameters[2];

            var generalDataFlowConfigration = new GeneralStreamFlowConfigration
            {
                ModuleConfigurations = new List<ModuleConfiguration>
                {
                    new ModuleConfiguration
                    {
                        Name = "msdn-metadata-module"
                    }
                },
                UseCache = false
            }; 

            var paginationDiscoveryConfigration = JsonConfigrationManager.Current.GetSetting(settingName);

            if(paginationDiscoveryConfigration == null)
            {
                throw new System.ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            Logger.Current.Info("Run general dataflow in init mode");

            IURIDiscovery discovery = new PaginationDiscovery(paginationDiscoveryConfigration);

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
