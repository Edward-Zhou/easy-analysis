using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.IO;
using EasyAnalysis.Modules;
using System.Collections.Generic;

namespace EasyAnalysis.Backend
{
    internal class DataflowExec
    {
        public void RunDataFlow(string[] parameters)
        {
            string settingName = parameters[0].ToLower();

            string type = parameters[1].ToLower();

            string cacheFolder = parameters[2];

            string outputCollectionName = parameters[3];

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

            IURIDiscovery discovery;

            var paginationDiscoveryConfigration = JsonConfigrationManager.Current.GetSetting(settingName);

            if(paginationDiscoveryConfigration == null)
            {
                throw new System.ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            if (type.Equals("monitor"))
            {
                Logger.Current.Info("Run general dataflow in monitor mode");

                discovery = new PeriodicPaginationDiscovery(paginationDiscoveryConfigration);
            }
            else
            {
                Logger.Current.Info("Run general dataflow in init mode");

                discovery = new PaginationDiscovery(paginationDiscoveryConfigration);
            }

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
