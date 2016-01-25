using System;
using System.Collections.Generic;

namespace EasyAnalysis.ScheduledTask
{
    public class ScheduledTask
    {
        public ITrigger Trigger { get; set; }

        public IEnumerable<CommandLineDefinition> Commands { get; set; }

        public void Run()
        {
            foreach(var command in Commands)
            {
                var arguments = command.Arguments;

                var beginOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var beginOfNextMonth = beginOfThisMonth.AddMonths(1);

                DateTime today = DateTime.Today;

                var variables = new Dictionary<string, string>
                    {
                        {"today", today.ToString("yyyy-MM-ddThh:mm:ss")},
                        {"tomorrow", today.AddDays(1).ToString("yyyy-MM-ddThh:mm:ss") },
                        { "$dateAdd(today, 2)", today.AddDays(2).ToString("yyyy-MM-ddThh:mm:ss")},
                        { "$dateAdd(today, -2)", today.AddDays(-2).ToString("yyyy-MM-ddThh:mm:ss")},
                        {"begin_of_this_month", beginOfThisMonth.ToString("yyyy-MM-ddThh:mm:ss") },
                        {"begin_of_next_month", beginOfNextMonth.ToString("yyyy-MM-ddThh:mm:ss") }
                    };

                foreach (var placeholder in variables)
                {
                    arguments = arguments.Replace("{{" + placeholder.Key + "}}", placeholder.Value);
                }

                System.Diagnostics.Process process = new System.Diagnostics.Process();

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                var applicationExeFile = command.Application;

                startInfo.FileName = applicationExeFile;

                startInfo.Arguments = arguments;

                process.StartInfo = startInfo;

                process.Start();
            }
        }
    }
}
