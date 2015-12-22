using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.ScheduledTask
{
    public class TriggerDefinition
    {
        public string Name { get; set; }

        public string[] Constructor { get; set; }

        public string Application { get; set; }

        public string Arguments { get; set; }
    }
}
