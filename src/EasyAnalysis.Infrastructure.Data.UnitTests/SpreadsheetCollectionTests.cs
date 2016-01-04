using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace EasyAnalysis.Data.Tests
{
    [TestClass()]
    public class SpreadsheetCollectionTests
    {
        [TestMethod()]
        public void ForEachAsyncTest()
        {


            var collection = new SpreadsheetCollection("Outputcat.xlsx");

            var result = new List<BsonDocument>();

            var task = collection.ForEachAsync((item) => {
                result.Add(item);
            });

            task.Wait();

            Assert.AreEqual(100, result.Count);

            var first = result.FirstOrDefault();

            var title = first.GetValue("Title").AsString;

            var caseNumber = first.GetValue("Case Number").AsString;

            Assert.AreEqual("[WP8.1] -Performance issue in WP8.1 platform ARM", title);

            Assert.AreEqual("CAS-4000266-R2G3G1", caseNumber);
        }
    }
}