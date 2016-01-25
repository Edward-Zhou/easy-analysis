using System;
using System.Collections.Generic;
using System.Timers;

namespace EasyAnalysis.ScheduledTask
{
    public class ScheduleContext
    {
        public DateTime SignalTime {get; set;}
    }

    public class ScheduleEngine
    {
        private IList<ScheduledTask> _tasks;

        public ScheduleEngine()
        {
            _tasks = new List<ScheduledTask>();
        }

        public void AddTask(ScheduledTask task)
        {
            _tasks.Add(task);
        }

        public void Run()
        {
            var coreTimer = new Timer(1000);

            coreTimer.Elapsed += OnTimedEvent;

            coreTimer.AutoReset = true;

            coreTimer.Enabled = true;

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var context = new ScheduleContext {  SignalTime = e.SignalTime };

            foreach (var task in _tasks)
            {
                if(task.Trigger.IsMatch(context))
                {
                    task.Run();
                }
            }
        }
    }
}
