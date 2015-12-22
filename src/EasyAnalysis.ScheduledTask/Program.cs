using EasyAnalysis.Actions;
using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EasyAnalysis.ScheduledTask
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-interval seconds]: 60 = 1 minutes
        /// [... additional parameters]: type:package name:c:\config\uwp-seq.json
        /// </param>
        static void Main(string[] args)
        {
            var intervalSeconds = int.Parse(args[0]);

            var trigger = new IntervalTimeTrigger(intervalSeconds, (lastTriggerTime, thisTriggerTime) => {

                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var endOfMonth = startOfMonth.AddMonths(1);


                var parameters = string.Format(
                    "parameters:start={0:yyyy-MM-ddThh:mm:ssZ}&end={1:yyyy-MM-ddThh:mm:ssZ}&this_month_start={2:yyyy-MM-ddThh:mm:ssZ}&this_month_end={3:yyyy-MM-ddThh:mm:ssZ}", lastTriggerTime, thisTriggerTime, startOfMonth, endOfMonth);

                System.Diagnostics.Process process = new System.Diagnostics.Process();

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                var applicationExeFile = System.Configuration.ConfigurationManager.AppSettings.Get("application");

                startInfo.FileName = applicationExeFile;

                startInfo.WorkingDirectory = Path.GetDirectoryName(applicationExeFile);

                var arguments = args.Skip(1).ToList();

                arguments.Add(parameters);

                startInfo.Arguments = string.Join(" ", arguments);

                process.StartInfo = startInfo;

                process.Start();
            });


            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
