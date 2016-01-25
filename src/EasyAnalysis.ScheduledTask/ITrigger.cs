using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.ScheduledTask
{
    public interface ITrigger
    {
        bool IsMatch(ScheduleContext context);
    }
}
