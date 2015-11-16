using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Cache;
using EasyAnalysis.Framework.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasyAnalysis.Framework
{
    public class GeneralDataFlowConfigration
    {
        public GeneralDataFlowConfigration()
        {
            // set default values
            UseCache = true;
        }

        public IEnumerable<ModuleConfiguration> ModuleConfigurations { get; set; }

        public bool UseCache { get; set; }
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

                Logger.Current.Info(string.Format("Load module [{0}]", moduleConfig.Name));
            }

            _modules = modules;
        }

        private void OnUriDiscovered(string url)
        {
            Logger.Current.Info(string.Format("Discovered [{0}]", url));

            var cacheClient = _cacheService.CreateClient();

            var status = cacheClient.GetStatus(new Uri(url));

            if(status != CacheStatus.Active || !_config.UseCache)
            {
                var httpClient = new HttpClient();

                var task = httpClient.GetStreamAsync(url);

                task.Wait();

                cacheClient.SetCache(new Uri(url), task.Result);
            }

            using (var cache = cacheClient.GetCache(new Uri(url)))
            {
                var metadata = new Dictionary<string, object>();

                foreach(var module in _modules)
                {
                    module.OnProcess(metadata, cache);
                }

                if(_output != null)
                {
                    _output.Output(metadata);
                }
            }            
        }
    }
}
