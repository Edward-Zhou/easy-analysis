using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class TimeFrameRange
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public static TimeFrameRange Parse(string text)
        {
            var temp = text.Split('&');

            var start = temp[0];

            var end = temp[1];


            return new TimeFrameRange
            {
                Start = DateTime.Parse(start),
                End = DateTime.Parse(end)
            };
        }

        public static string ParseBack(TimeFrameRange tfr)
        {
            return tfr.Start.ToString("yyyy-MM-ddT00:00:00Z") + "&" + tfr.End.ToString("yyyy-MM-ddT00:00:00Z");
        }
    }
}
