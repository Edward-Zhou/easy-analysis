using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Analysis
{
    public class ModuleConfiguration
    {
        public string Name { get; set; }

        public IEnumerable<string> Parameters { get; set; }
    }
}
