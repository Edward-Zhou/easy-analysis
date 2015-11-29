using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class SuggestTags : IAction
    {
        public string Description
        {
            get
            {
                return "auto-suggest tags to a question/thread";
            }
        }

        public Task RunAsync(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
