using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Discovery
{
    public class PaginationDiscovery : IURIDiscovery
    {
        public event Action<string> OnNew;

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
