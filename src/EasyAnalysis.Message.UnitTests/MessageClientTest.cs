using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Message.UnitTests
{
    [TestClass]
    public class MessageClientTest
    {
        [TestMethod]
        public void SendMsgTest()
        {
            var client = new MessageClient("import-new-question");

            var importNewQuestionCmd = new Command.ImportQuestionCommand
            {
                Url = "https://social.msdn.microsoft.com/Forums/en-US/dc0c24b5-6aa7-4d09-aabb-8cf5f24ba1f8/wp81-webview-script-notify-event-is-not-firing-for-msappdatamslocalstream-in-windows-phone?forum=wpdevelop",
                Collection = "landing.threads"
            };

            client.Send(importNewQuestionCmd);
        }
    }
}
