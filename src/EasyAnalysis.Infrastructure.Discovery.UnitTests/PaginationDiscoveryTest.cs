using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EasyAnalysis.Infrastructure.Discovery.UnitTests
{
    [TestClass]
    public class PaginationDiscoveryTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var configration = new PaginationDiscoveryConfigration
            {
                UrlFormat = "https://social.msdn.microsoft.com/Forums/windowsapps/en-US/home?forum=wpdevelop&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page={0}",
                Start = 1,
                Length = 1,
                Encoding = "utf-8",
                LookUp = new XPathAttributeLookUp
                {
                    XPath = "//*[@id=\"threadList\"]/li/div/a",
                    Attribute = "href"
                },
                Transform = new RegexTransform
                {
                    Pattern = "(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})",
                    Expression = "http://social.msdn.microsoft.com/Forums/en-US/{1}?outputAs=xml"
                },
                Filter = "(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})"
            };

            var discoery = new PaginationDiscovery(configration);

            var list = new List<string>();

            discoery.OnDiscovered += (url) => {
                list.Add(url);
            };

            discoery.Start();

            Assert.AreEqual(20, list.Count);
        }
    }
}
