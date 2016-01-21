using EasyAnalysis.Message;
using System;

namespace EasyAnalysis.Modules
{
    public class MessageHandlerFactory
    {
        public IMessageHandler Activate(string name)
        {
            switch(name.ToLower())
            {
                case "import-msdn-thread": return new ImportMsdnAndTechNetThreadHandler();
                case "sync-up-with-webdb": throw  new NotImplementedException();
                default: throw new NotImplementedException();
            }
        }
    }
}
