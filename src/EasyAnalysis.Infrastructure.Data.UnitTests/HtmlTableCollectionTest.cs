using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Data;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Linq;

namespace EasyAnalysis.Infrastructure.Data.UnitTests
{
    [TestClass]
    public class HtmlTableCollectionTest
    {
        [TestMethod]
        public void LoadDataTest()
        {
            var collection = new HtmlTableCollection("html_collection.html", "//*[@id='gridBodyTable']/tr");

            var result = new List<BsonDocument>();

            var task = collection.ForEachAsync((item) => {
                result.Add(item);
            });

            task.Wait();

            Assert.AreEqual(100, result.Count);

            var first = result.FirstOrDefault();

            var externalId = first.GetValue("External ID (Thread)").AsString;

            var owner = first.GetValue("Owner").AsString;

            Assert.AreEqual("ec36df8b-2209-4a6e-ba18-fc4f835a9f09", externalId);

            Assert.AreEqual("Fang Peng", owner);
        }
    }
}
