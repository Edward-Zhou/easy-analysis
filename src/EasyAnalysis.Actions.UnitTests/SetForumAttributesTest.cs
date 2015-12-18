using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Actions.UnitTests
{
    [TestClass]
    public class SetForumAttributesTest
    {
        [TestMethod]
        public void RunTest()
        {
            var action = new SetForumAttributes();

            var task = action.RunAsync(new string[] {
                "uwp.thread_profiles",
                "2015-12-01T00:00:00Z&2015-12-18T00:00:00Z"});

            task.Wait();
        }
    }
}
