using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    public class DefaultWebStreamFlowFactory
    {
        public IWebStreamFlow Activate(string name)
        {
            switch (name)
            {
                case "msdn-monitor": return new MsdnMonitorWebStreamFlow();
                case "msdn-init": return new MsdnInitWebStreamFlow();
                case "msdn-refresh": return new MsdnRefreshWebStreamFlow();
                case "stackoverflow-import": return new ImportStackOverflowDataFlow();
                default: return null;
            }
        }
    }
}
