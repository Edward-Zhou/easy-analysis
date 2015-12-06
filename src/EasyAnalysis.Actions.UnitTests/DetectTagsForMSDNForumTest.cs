using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Framework.Analysis;

namespace EasyAnalysis.Actions.UnitTests
{
    [TestClass]
    public class DetectTagsForMSDNForumTest
    {
        [TestMethod]
        public void RunWithTimeFrameTest()
        {
            IAction action = new DetectTagsForMSDNForum();

            var task = action.RunAsync(new string[] {
                "landing.threads",
                "2015-12-01T00:00:00Z&2015-12-06T00:00:00Z" });

            task.Wait();
        }
    }
}
