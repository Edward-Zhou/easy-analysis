using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class SyncWithStackoverflow : IAction
    {
        public string Description
        {
            get
            {
                return "Sync up stackoverflow data with external system";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-timeframe      (optional), e.g. 2015-10-01T00:00:00&2015-11-30T00:00:00]
        /// </param>
        /// <returns></returns>
        public Task RunAsync(string[] args)
        {
            // query the exteranl database

            // IF NOT EXISTS
            // import the data to EAS web database
            // ELSE SKIP

            // ID FORMAT: SO_{QUESTION_ID}

            // FORUM_ID: stackoverflow.uwp

            throw new NotImplementedException();
        }
    }
}
