using System;
using System.IO;
using msmq = System.Messaging;

namespace EasyAnalysis.Message
{
    public class MessageListener : IDisposable
    {
        public event Action<string> OnReceived;

        private msmq.MessageQueue _queue;

        public MessageListener(string name)
        {
            _queue = new msmq.MessageQueue(".\\Private$\\" + name);
        }

        public void Listen()
        {
            while (true)
            {
                var msg = _queue.Receive();

                using (var sr = new StreamReader(msg.BodyStream))
                {
                    var json = sr.ReadToEnd();

                    if(OnReceived != null)
                    {
                        OnReceived.Invoke(json);
                    }
                }
            }
        }

        public void Dispose()
        {
            _queue.Dispose();
        }
    }
}
