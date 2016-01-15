using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Reporting.UnitTests
{
    [TestClass]
    public class CategoryReportObjectTest
    {
        [TestMethod]
        public void BasicRunTest()
        {
            var obj = new CategoryReportObject();

            var elements = obj.Run("UWP", DateTime.Parse("2015-12-1"), DateTime.Parse("2016-1-1"));

            Assert.AreEqual(9, elements.Count());
        }
    }
}
