using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Actions.UnitTests
{
    [TestClass]
    public class SyncUpDataTest
    {
        [TestMethod]
        public void SyncupForumId()
        {
            var action = new SyncUpData();

            var task = action.RunAsync(new string[] { "landing.threads", "2016-01-01T00:00:00&2016-02-01T00:00:00" });

            task.Wait();
        }
    }
}
