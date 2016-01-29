namespace EasyAnalysis.ScheduledTask
{
    public class IntervalTimeTrigger : ITrigger
    {
        private int _intervalSeconds;

        public IntervalTimeTrigger(int intervalSecond)
        {
            _intervalSeconds = intervalSecond;
        }

        public bool IsMatch(ScheduleContext context)
        {
            return (int)(context.SignalTime.TimeOfDay.TotalSeconds) % _intervalSeconds == 0;
        }
    }
}
