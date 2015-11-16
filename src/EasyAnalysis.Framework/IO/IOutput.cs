using System.Collections.Generic;

namespace EasyAnalysis.Framework.IO
{
    public interface IOutput
    {
        void Output(IDictionary<string, object> data);
    }
}
