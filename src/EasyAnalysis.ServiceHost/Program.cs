using EasyAnalysis.Framework;
using EasyAnalysis.Message;
using EasyAnalysis.Modules;
using System;

namespace EasyAnalysis.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new MessageListener("import-new-question");

            var handler = new ImportMsdnAndTechNetThreadHandler();

            listener.OnReceived += (body) => {
                try
                {
                    handler.Handle(body);
                }catch(Exception ex)
                {
                    Logger.Current.Error(ex.Message);
                }
            };

            listener.Listen();
        }
    }
}
