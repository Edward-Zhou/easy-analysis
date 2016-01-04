using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Framework.Config;

namespace EasyAnalysis.Framework.UnitTests
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
        public string Name { get; set; }

        public string UrlFormat { get; set; }

        public string BaseUri { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public string Encoding { get; set; }

        public string Filter { get; set; }

        public XPathAttributeLookUp LookUp { get; set; }

        public RegexTransform Transform { get; set; }
    }

    [TestClass]
    public class JsonConfigrationTest
    {
        [TestMethod]
        public void OverrideTest()
        {
            var config = new JsonConfigrationManager<PaginationDiscoveryConfigration>("tasks.json");

            var setting = config.GetSetting("uwp_sort_by_post");

            Assert.AreEqual("UWP threads sort by post", setting.Name);

            Assert.AreEqual(1, setting.Start);

            Assert.AreEqual(100, setting.Length);
        }
    }
}
