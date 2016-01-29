using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.ScheduledTask
{

    public class ScheduledTaskDefinition
    {
        public string Name { get; set; }

        public TriggerDefinition Trigger { get; set; }

        public IEnumerable<CommandLineDefinition> Commands { get; set; }
    }
}
