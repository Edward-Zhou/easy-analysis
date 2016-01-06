using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Cache;
using EasyAnalysis.Framework.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasyAnalysis.Framework
{
    public class GeneralStreamFlowConfigration
    {
        public GeneralStreamFlowConfigration()
        {
            UseCache = true;
        }

        public IEnumerable<string> ProcessModules { get; set; }

        public bool UseCache { get; set; }
    }

    public class GeneralStreamFlow
    {
        public event Action<IDictionary<string, object>> OnFail;

        ICacheService _cacheService;

        IResourceDiscovery _uriDiscovery;

        IMetadataProcessModuleFactory _moduleFactory;

        IOutput _output;

        private StreamProcessingPipeline _streamProcessingPipeline;

        private readonly GeneralStreamFlowConfigration _config;

        public GeneralStreamFlow(
            GeneralStreamFlowConfigration config,
            IResourceDiscovery uriDiscovery, 
            IMetadataProcessModuleFactory moduleFactory,
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
            InitComponents();
        }

        public void Run()
        {
            _uriDiscovery.OnDiscovered += OnUriDiscovered;

            _uriDiscovery.Start();
        }

        private void InitComponents()
        {
            var modules = new List<IMetadataProcessModule>();

            foreach(var moduleName in _config.ProcessModules)
            {
                var moduleToLoad = _moduleFactory.Activate(moduleName);

                modules.Add(moduleToLoad);

                Logger.Current.Info(string.Format("Load module [{0}]", moduleName));
            }

            _streamProcessingPipeline = new StreamProcessingPipeline(modules);

            _streamProcessingPipeline.OnOutput += (metadata) => {
                if (_output != null && metadata.Count > 0)
                {
                    _output.Output(metadata);
                }
            };
        }

        private void OnUriDiscovered(string url)
        {
            try
            {
                Logger.Current.Info(string.Format("Discovered [{0}]", url));

                var cacheClient = _cacheService.CreateClient();

                var status = cacheClient.GetStatus(new Uri(url));

                if (status != CacheStatus.Active || !_config.UseCache)
                {
                    var httpClient = new HttpClient();

                    var task = httpClient.GetStreamAsync(url);

                    task.Wait();

                    cacheClient.SetCache(new Uri(url), task.Result);
                }

                using (var cache = cacheClient.GetCache(new Uri(url)))
                {
                    _streamProcessingPipeline.Process(cache);
                }
            }
            catch(Exception ex)
            {
                OnFail(new Dictionary<string, object> {
                    { "url", url}
                });

                Logger.Current.Error(string.Format("URL[{0}], ERROR: {1}", url, ex.Message));
            }
        }
    }
}
