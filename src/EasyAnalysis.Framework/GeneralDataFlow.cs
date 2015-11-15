using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public class GeneralDataFlow
    {
        ICacheService _cacheService;

        IActionFactory _actionFactory;

        IURIDiscovery _uriDiscovery;

        public GeneralDataFlow(IURIDiscovery uriDiscovery, ICacheService cacheServcie, IActionFactory actionFactory)
        {
            _uriDiscovery = uriDiscovery;

            _cacheService = cacheServcie;

            _actionFactory = actionFactory;
        }

        public void Run()
        {
            _uriDiscovery.OnDiscovered += OnUriDiscovered;

            _uriDiscovery.Start();
        }

        private void OnUriDiscovered(string url)
        {
            
        }

        private void OnChange()
        {

        }
    }
}
