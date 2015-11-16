namespace EasyAnalysis.Infrastructure.Discovery
{
    public class RegexTransform
    {
        public string Pattern { get; set; }

        public string Expression { get; set; }
    }

    public class XPathAttributeLookUp
    {
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

        public string Filter { get; set; }

        public XPathAttributeLookUp LookUp { get; set; }

        public RegexTransform Transform { get; set; }
    }
}
