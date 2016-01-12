using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Message.UnitTests
{
    [TestClass]
    public class MessageListenerTest
    {
        [TestMethod]
        public void ReceiveMsgTest()
        {
            var listener = new MessageListener("import-new-question");

            listener.OnReceived += (body) => {
                var cmd = Newtonsoft.Json.JsonConvert.DeserializeObject<Command.ImportQuestionCommand>(body);
            };

            listener.Listen();
        }
    }
}
