using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EasyAnalysis.ScheduledTask
{
    public class IntervalTimeTrigger
    {
        private DateTime _lastTriggerTime;

        private int _intervalSeconds;

        private Action<DateTime, DateTime> _onTrigger;

        private Timer _timer;

        public IntervalTimeTrigger(int intervalSeconds, Action<DateTime, DateTime> onTrigger)
        {
            _intervalSeconds = intervalSeconds;

            _onTrigger = onTrigger;

            _timer = new Timer(_intervalSeconds * 1000);
            
            _timer.Elapsed += OnTimedEvent;

            _timer.AutoReset = true;

            _timer.Enabled = true;

            _lastTriggerTime = DateTime.UtcNow;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var temp = DateTime.UtcNow;

            if (_onTrigger != null)
            {
                try
                {
                    _onTrigger(_lastTriggerTime, temp);
                }
                catch (Exception ex)
                {
                    // innore the exception
                }
            }

            _lastTriggerTime = temp;
        }
    }
}
