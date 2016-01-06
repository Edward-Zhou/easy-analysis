using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyAnalysis.Framework
{
    public class StreamProcessingPipeline
    {
        private IEnumerable<IMetadataProcessModule> _modules;

        public StreamProcessingPipeline(IEnumerable<IMetadataProcessModule> modules)
        {
            _modules = modules;
        }

        public event Action<IDictionary<string, object>> OnOutput;

        public void Process(Stream stream)
        {
            var metadata = new Dictionary<string, object>();

            foreach(var module in _modules)
            {
                module.OnProcess(metadata, stream);
            }

            OnOutput(metadata);
        }
    }
}
