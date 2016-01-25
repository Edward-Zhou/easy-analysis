using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyAnalysis.ScheduledTask
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        ///
        /// </param>
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("Please specify the task definination file path. e.g. task_def.json");
            }

            var taskDefininationFilePath = args[0];

            var text = File.ReadAllText(taskDefininationFilePath);

            var taskDefinations = JsonConvert.DeserializeObject<List<ScheduledTaskDefinition>>(text, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var engine = new ScheduleEngine();

            foreach(var taskDef in taskDefinations)
            {
                engine.AddTask(ContructTaskFromDefinination(taskDef));
            }

            engine.Run();
        }


        static ScheduledTask ContructTaskFromDefinination(ScheduledTaskDefinition scheduledTaskDefinition)
        {
            var task = new ScheduledTask();

            task.Commands = scheduledTaskDefinition.Commands;

            task.Trigger = new IntervalTimeTrigger(int.Parse(scheduledTaskDefinition.Trigger.Constructor[0]));

            return task;
        }
    }
}
