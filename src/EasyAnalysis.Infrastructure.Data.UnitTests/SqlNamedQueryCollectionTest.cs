using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Data;
using System.Collections.Generic;

namespace EasyAnalysis.Infrastructure.Data.UnitTests
{
    public class TestModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn {get; set;}

        public string Category { get; set;}

        public string @Type { get; set; }

        public string Repository { get; set; }
    }

    [TestClass]
    public class SqlNamedQueryCollectionTest
    {
        [TestMethod]
        public void ForEachTest()
        {
            var collection = new SqlNamedQueryCollection<TestModel>("DefaultConnection", "get_thread_profile");

            collection.SetParameters(new
            {
                repository = "UWP",
                start = DateTime.Parse("2016-1-1"),
                end = DateTime.Parse("2016-1-2")
            });

            var list = new List<TestModel>();

            var task = collection.ForEachAsync((model) => {
                list.Add(model);
            });

            task.Wait();
        }
    }
}
