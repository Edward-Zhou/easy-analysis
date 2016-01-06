using EasyAnalysis.Framework.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasyAnalysis.Framework
{
    public class DiscoveryStreamFlow
    {
        IResourceDiscovery _uriDiscovery;

        private StreamProcessingPipeline _streamProcessingPipeline;

        IOutput _output;

        public DiscoveryStreamFlow(
            IResourceDiscovery uriDiscovery, 
            StreamProcessingPipeline pipeline,
            IOutput output)
        {
            _uriDiscovery = uriDiscovery;

            _streamProcessingPipeline = pipeline;

            _output = output;
        }


        public void Run()
        {
            _uriDiscovery.OnDiscovered += OnUriDiscovered;

            _streamProcessingPipeline.OnOutput += OnPipelineOutput;

            _uriDiscovery.Start();
        }

        private void OnPipelineOutput(IDictionary<string, object> metadata)
        {
            if (_output != null && metadata.Count > 0)
            {
                _output.Output(metadata);
            }
        }

        private void OnUriDiscovered(string url)
        {
            try
            {
                Logger.Current.Info(string.Format("Discovered [{0}]", url));

                var httpClient = new HttpClient();

                var task = httpClient.GetStreamAsync(url);

                task.Wait();

                var content = task.Result;

                _streamProcessingPipeline.Process(content);
            }
            catch (Exception ex)
            {
                Logger.Current.Error(string.Format("URL[{0}], ERROR: {1}", url, ex.Message));
            }
        }
    }
}
