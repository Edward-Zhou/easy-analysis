using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.IO;
using EasyAnalysis.Message;
using EasyAnalysis.Message.Command;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasyAnalysis.Modules
{
    public class ImportMsdnAndTechNetThreadHandler : IMessageHandler
    {
        private DefaultModuleFactory _factory;

        public ImportMsdnAndTechNetThreadHandler()
        {
            _factory = new DefaultModuleFactory();
        }

        public void Handle(string body)
        {
            var cmd = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportQuestionCommand>(body);

            var modules = new List<IMetadataProcessModule>();

            modules.Add(_factory.Activate("msdn-metadata-module"));

            IOutput output = new Infrastructure.IO.MongoCollectionOutput(cmd.Collection);

            var streamProcessingPipeline = new StreamProcessingPipeline(modules);

            streamProcessingPipeline.OnOutput += (metadata) => {
                if (metadata.Count > 0)
                {
                    Logger.Current.Info("output the meatadata");

                    output.Output(metadata);
                }
            };

            try
            {
                var httpClient = new HttpClient();

                var task = httpClient.GetStreamAsync(cmd.Url);

                task.Wait();

                Logger.Current.Info(string.Format("load content successfully [{0}]", cmd.Url));

                streamProcessingPipeline.Process(task.Result);
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);

                throw;
            }
        }
    }
}
