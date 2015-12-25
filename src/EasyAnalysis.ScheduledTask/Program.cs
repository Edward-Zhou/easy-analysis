using EasyAnalysis.Actions;
using EasyAnalysis.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            IList<IntervalTimeTrigger> triggers = new List<IntervalTimeTrigger>();

            var text = File.ReadAllText("triggers.json");

            var triggerDefinations = JsonConvert.DeserializeObject<List<TriggerDefinition>>(text, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            foreach (var triggerDefination in triggerDefinations)
            {
                var trigger = new IntervalTimeTrigger(
                int.Parse(triggerDefination.Constructor[0]),
                (lastTriggerTime, thisTriggerTime) =>
                {
                    var beginOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    var beginOfNextMonth = beginOfThisMonth.AddMonths(1);

                    var palceholders = new Dictionary<string, string>
                    {
                        {"this_trigger_time", thisTriggerTime.ToString("yyyy-MM-ddThh:mm:ssZ") },
                        {"last_trigger_time", lastTriggerTime.ToString("yyyy-MM-ddThh:mm:ssZ") },
                        {"begin_of_this_month", beginOfThisMonth.ToString("yyyy-MM-ddThh:mm:ssZ") },
                        {"begin_of_next_month", beginOfNextMonth.ToString("yyyy-MM-ddThh:mm:ssZ") }
                    };

                    var arguments = triggerDefination.Arguments;

                    foreach (var placeholder in palceholders)
                    {
                        arguments = arguments.Replace("{{" + placeholder.Key + "}}", placeholder.Value);
                    }

                    System.Diagnostics.Process process = new System.Diagnostics.Process();

                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                    var applicationExeFile = triggerDefination.Application;

                    startInfo.FileName = applicationExeFile;

                    startInfo.WorkingDirectory = Path.GetDirectoryName(applicationExeFile);

                    startInfo.Arguments = arguments;

                    process.StartInfo = startInfo;

                    process.Start();
                });

                triggers.Add(trigger);
            }

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
