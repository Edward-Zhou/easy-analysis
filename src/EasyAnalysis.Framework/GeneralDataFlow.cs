using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Cache;
using EasyAnalysis.Framework.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public class GeneralDataFlowConfigration
    {
        public IEnumerable<ModuleConfiguration> ModuleConfigurations { get; set; }

        public IEnumerable<string> Actions { get; set; }
    }

    public class GeneralDataFlow
    {
        ICacheService _cacheService;

        IURIDiscovery _uriDiscovery;

        IModuleFactory _moduleFactory;

        IOutput _output;

        IEnumerable<IModule> _modules;

        private readonly GeneralDataFlowConfigration _config;

        public GeneralDataFlow(
            GeneralDataFlowConfigration config,
            IURIDiscovery uriDiscovery, 
            IModuleFactory moduleFactory,
            ICacheService cacheServcie, 
            IOutput output)
        {
            _uriDiscovery = uriDiscovery;

            _cacheService = cacheServcie;

            _moduleFactory = moduleFactory;

            _output = output;

            _config = config;
        }

        public void Init()
        {
            LoadModules();
        }

        public void Run()
        {
            _uriDiscovery.OnDiscovered += OnUriDiscovered;

            _uriDiscovery.Start();
        }

        private void LoadModules()
        {
            var modules = new List<IModule>();

            foreach(var moduleConfig in _config.ModuleConfigurations)
            {
                var moduleToLoad = _moduleFactory.CreateInstance(moduleConfig.Name);

                moduleToLoad.Init(moduleConfig.Parameters);

                modules.Add(moduleToLoad);
            }

            _modules = modules;
        }

        private void OnUriDiscovered(string url)
        {
            var client = _cacheService.CreateClient();

            var status = client.GetStatus(new Uri(url));

            if(status != CacheStatus.Active)
            {
                // HTTP Client Require the cotent and set the cache
            }

            using (var cache = client.GetCache(new Uri(url)))
            {
                var metadata = new Dictionary<string, object>();

                foreach(var module in _modules)
                {
                    module.OnProcess(metadata, cache);
                }

                _output.Output(metadata);
            }            
        }
    }
}
