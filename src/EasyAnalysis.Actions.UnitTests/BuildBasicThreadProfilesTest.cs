using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Actions.UnitTests
{
    [TestClass]
    public class BuildBasicThreadProfilesTest
    {
        [TestMethod]
        public void RunTest()
        {
            var action = new BuildBasicThreadProfiles(new Data.UniversalConnectionStringProvider());

            var task = action.RunAsync(new string[] {
                      "landing.threads",
                      "uwp.thread_profiles",
                      "2015-12-17T00:00:00Z&2015-12-18T00:00:00Z"
            });

            task.Wait();
        }
    }
}
