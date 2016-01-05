using System.Collections.Generic;
using System.IO;

namespace EasyAnalysis.Framework.Analysis
{
    public interface IMetadataProcessModule
    {
        void OnProcess(IDictionary<string, object> metadata, Stream stream);
    }
}
