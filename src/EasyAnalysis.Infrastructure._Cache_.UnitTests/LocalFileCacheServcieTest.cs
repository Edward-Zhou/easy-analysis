using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using EasyAnalysis.Framework.Cache;

namespace EasyAnalysis.Infrastructure.Cache.UnitTests
{
    [TestClass]
    public class LocalFileCacheServcieTest
    {
        [TestMethod]
        public void CreateClientTest()
        {
            var dbFilePath = @"D:\forum_cache\index.sqlite3";

            if (File.Exists(dbFilePath))
            {
                File.Delete(dbFilePath);
            }

            var service = new LocalFileCacheServcie();

            service.Configure(@"D:\forum_cache");

            var client = service.CreateClient();
        }

        [TestMethod]
        public void AddOrUpdateCacheTest()
        {
            ICacheClient client = CreateTestClient();

            using (var content = new MemoryStream())
            using (var sw = new StreamWriter(content))
            {
                sw.WriteLine("hello world!");

                sw.Flush();

                content.Position = 0;

                client.SetCache(new Uri("http://www.localhost.com"), content);

                var cache = client.GetCache(new Uri("http://www.localhost.com"));

                Assert.IsNotNull(cache);

                var sr = new StreamReader(cache);

                var contentInCache = sr.ReadLine();

                Assert.AreEqual("hello world!", contentInCache);
            }
        }

        [TestMethod]
        public void GetCacheStatusTest()
        {
            ICacheClient client = CreateTestClient();

            var noneStatus = client.GetStatus(new Uri("http://www.none.com"));

            Assert.AreEqual(CacheStatus.None, noneStatus);

            using (var content = new MemoryStream())
            using (var sw = new StreamWriter(content))
            {
                sw.WriteLine("hello world!");

                sw.Flush();

                content.Position = 0;

                client.SetCache(new Uri("http://www.localhost.com/new"), content);

                var activeStatus = client.GetStatus(new Uri("http://www.localhost.com/new"));

                Assert.AreEqual(CacheStatus.Active, activeStatus);
            }

        }

        private static ICacheClient CreateTestClient()
        {
            var service = new LocalFileCacheServcie();

            service.Configure(@"D:\forum_cache");

            var client = service.CreateClient();
            return client;
        }
    }
}
