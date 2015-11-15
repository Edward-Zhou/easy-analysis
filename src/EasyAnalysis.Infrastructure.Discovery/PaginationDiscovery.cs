using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Discovery
{
    public class XPathAttributeLookUp {
        public string XPath { get; set; }
        public string Attribute { get; set; }
    }

    public class PaginationDiscoveryConfigration
    {
        public string UrlFormat { get; set; }

        public string BaseUri { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public string Encoding { get; set; }

        public XPathAttributeLookUp LookUp { get; set; }
    }

    public class PaginationDiscovery : IURIDiscovery
    {
        private readonly PaginationDiscoveryConfigration _config;

        public PaginationDiscovery(PaginationDiscoveryConfigration config)
        {
            _config = config;
        }

        public event Action<string> OnDiscovered;

        public void Start()
        {
            var task = Dsicover();

            task.Wait();

            throw new NotImplementedException();
        }

        private async Task Dsicover()
        {
            var navigation = new PageNavigation(_config.UrlFormat);

            int start = _config.Start;

            int end = _config.Start + _config.Length;

            var encoding = Encoding.GetEncoding(_config.Encoding);

            for (int i = start; i < end; i++)
            {
                navigation.NavigateTo(i);

                string text = string.Empty;

                using (var content = await navigation.GetAsync())
                using (var sr = new StreamReader(content, encoding))
                {
                    text = await sr.ReadToEndAsync();

                    // TODO: implement the discovery
                }
            }
        }
    }
}
