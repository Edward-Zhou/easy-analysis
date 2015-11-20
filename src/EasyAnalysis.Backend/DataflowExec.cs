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
            bool monitor = parameters[0].ToLower() == "monitor";

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

            var paginationDiscoveryConfigration = new PaginationDiscoveryConfigration
            {
                UrlFormat = "https://social.msdn.microsoft.com/Forums/windowsapps/en-US/home?forum=wpdevelop&filter=alltypes&sort=lastpostdesc&brandIgnore=true&page={0}",
                Start = 1,
                Length = 1,
                Encoding = "utf-8",
                LookUp = new XPathAttributeLookUp
                {
                    XPath = "//*[@id=\"threadList\"]/li/div/a",
                    Attribute = "href"
                },
                Transform = new RegexTransform
                {
                    Pattern = "(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})",
                    Expression = "http://social.msdn.microsoft.com/Forums/en-US/{1}?outputAs=xml"
                },
                Filter = "(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})"
            };

            IURIDiscovery discovery;

            if (monitor)
            {
                Logger.Current.Info("Run general dataflow in monitor mode");

                discovery = new PeriodicPaginationDiscovery(paginationDiscoveryConfigration);
            }
            else
            {
                Logger.Current.Info("Run general dataflow in init mode");

                paginationDiscoveryConfigration.UrlFormat = "https://social.msdn.microsoft.com/Forums/windowsapps/en-US/home?forum=wpdevelop&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page={0}";

                paginationDiscoveryConfigration.Length = 100;

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
