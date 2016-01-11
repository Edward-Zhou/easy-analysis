using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasyAnalysis.Modules
{
    public class ImportMsdnAndTechNetThreadHandler : IHandler
    {
        public void OnProcess(IDictionary<string, object> context)
        {
            var factory = new DefaultModuleFactory();

            var modules = new List<IMetadataProcessModule>();

            modules.Add(factory.Activate("msdn-metadata-module"));

            IOutput output = new Infrastructure.IO.MongoCollectionOutput(context["output"] as string);

            var streamProcessingPipeline = new StreamProcessingPipeline(modules);

            streamProcessingPipeline.OnOutput += (metadata) => {
                if (metadata.Count > 0)
                {
                    output.Output(metadata);
                }
            };

            try
            {
                var httpClient = new HttpClient();

                var task = httpClient.GetStreamAsync(context["url"] as string);

                task.Wait();

                streamProcessingPipeline.Process(task.Result);
            }catch(Exception ex)
            {
                Logger.Current.Error(ex.Message);

                throw;
            }           
        }
    }
}
