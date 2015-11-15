using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Analysis
{
    public interface IModule
    {
        void Init(IEnumerable<string> arguments);

        void OnProcess(IDictionary<string, object> metadata, Stream stream);
    }
}
