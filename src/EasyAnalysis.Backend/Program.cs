using EasyAnalysis.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Modules;
using EasyAnalysis.Infrastructure.IO;

namespace EasyAnalysis.Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            RunDataFollow();
            // var steps = new List<Step>();

            //steps.Add(new Step {
            //    Action = "import-new-users",
            //    Parameters = new string[] {
            //        "uwp",
            //        "aug_threads",
            //        "users"
            //    }
            // });

            //steps.Add(new Step
            //{
            //    Action = "extract-user-activies",
            //    Parameters = new string[] {
            //        "uwp",
            //        "aug_threads"
            //    }
            //});

            //steps.Add(new Step
            //{
            //    Action = "build-thread-profiles",
            //    Parameters = new string[] {
            //        "uwp", // [repository]
            //        "2015-10-1", // [start date]
            //        "2015-10-31", // [end date]
            //        "uwp_latest", // [thread collection name]
            //        "uwp_oct_thread_profiles" // [target collection name]
            //    }
            //});

            //var factory = new DefaultActionFactory();

            //foreach(var step in steps)
            //{
            //    var action = factory.CreateInstance(step.Action);

            //    var task = action.RunAsync(step.Parameters);

            //    task.Wait();
            //}
        }

        static void RunDataFollow()
        {
            var cacheFolder = @"D:\forum_cache";

            var generalDataFlowConfigration = new GeneralDataFlowConfigration
            {
                ModuleConfigurations = new List<ModuleConfiguration>
                {
                    new ModuleConfiguration
                    {
                        Name = "msdn-metadata-module"
                    }
                }
            };

            var paginationDiscoveryConfigration = new PaginationDiscoveryConfigration
            {
                UrlFormat = "https://social.msdn.microsoft.com/Forums/windowsapps/en-US/home?forum=wpdevelop&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page={0}",
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

            PaginationDiscovery discovery = new PaginationDiscovery(paginationDiscoveryConfigration);

            var moduleFactory = new DefaultModuleFactory();

            var cacheService = new LocalFileCacheServcie();

            cacheService.Configure(cacheFolder);

            var output = new MongoCollectionOutput("landing:threads");

            var dataflow = new GeneralDataFlow(
                config:generalDataFlowConfigration, 
                uriDiscovery:discovery,
                moduleFactory: moduleFactory,
                cacheServcie: cacheService,
                output: output);

            dataflow.Init();

            dataflow.Run();
        }
    }
}
