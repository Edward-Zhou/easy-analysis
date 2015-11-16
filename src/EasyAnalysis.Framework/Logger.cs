using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public class Logger
    {
        public static Logger Current = new Logger();

        public void Info(string info)
        {
            Console.WriteLine(info);
        }

        public void Error(string err)
        {
            Console.WriteLine(err);
        }

        public void Warnning(string warnning)
        {
            Console.WriteLine(warnning);
        }
    }
}
