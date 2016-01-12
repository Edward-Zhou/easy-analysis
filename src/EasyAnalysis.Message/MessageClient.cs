using System;
using msmq = System.Messaging;
using System.Text;

namespace EasyAnalysis.Message
{
    public class MessageClient : IDisposable
    {
        private msmq.MessageQueue _queue;

        public MessageClient(string name)
        {
            _queue = new msmq.MessageQueue(".\\Private$\\" + name);
        }

        public void Dispose()
        {
            _queue.Dispose();
        }

        public void Send<T>(T body)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            using (var msg = new msmq.Message())
            using (var mem = new System.IO.MemoryStream(Encoding.Default.GetBytes(json)))
            {
                msg.BodyStream = mem;

                _queue.Send(msg);
            }
        }
    }
}
