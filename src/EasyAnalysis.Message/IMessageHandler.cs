using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Message
{
    public interface IMessageHandler
    {
        void Handle(string body);
    }
}
