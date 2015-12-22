using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Discovery
{
    public class ListDiscovery : IURIDiscovery
    {
        private IEnumerable<string> _list;

        public ListDiscovery(IEnumerable<string> list)
        {
            _list = list;
        }

        public event Action<string> OnDiscovered;

        public void Start()
        {
            foreach(var url in _list)
            {
                OnDiscovered(url);
            }
        }
    }
}
