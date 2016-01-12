using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Config;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.IO;
using EasyAnalysis.Modules;
using System.Collections.Generic;

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
                ProcessModules = new List<string>
                {
                    "msdn-metadata-module"
                },
                UseCache = false
            };

            IResourceDiscovery discovery = CreateDiscoveryBySettingName(settingName);

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

        private static IResourceDiscovery CreateDiscoveryBySettingName(string settingName)
        {
            var jsonConfigrationManager = new JsonConfigrationManager<PaginationDiscoveryConfigration>("config.json");

            var paginationDiscoveryConfigration = jsonConfigrationManager.GetSetting(settingName);

            if (paginationDiscoveryConfigration == null)
            {
                throw new System.ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            Logger.Current.Info("Run general dataflow in init mode");

            IResourceDiscovery discovery = new PaginationDiscovery(paginationDiscoveryConfigration);
            return discovery;
        }
    }
}
