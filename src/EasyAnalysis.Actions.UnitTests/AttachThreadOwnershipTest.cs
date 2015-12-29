using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Actions.UnitTests
{
    [TestClass]
    public class AttachThreadOwnershipTest
    {
        [TestMethod]
        public void AttachOwnershipFromHtmlFileTest()
        {
            var action = new AttachThreadOwnership();

            var task = action.RunAsync(new string[] { "html_collection.html", "uwp.thread_profiles" });

            task.Wait();
        }
    }
}
