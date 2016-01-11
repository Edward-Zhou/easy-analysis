using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Modules
{
    public interface IHandler
    {
        void OnProcess(IDictionary<string, object> context);
    }
}
