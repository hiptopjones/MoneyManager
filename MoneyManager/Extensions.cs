using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public static class Extensions
    {
        public static IEnumerable<DateTime> EachDay(this DateTime start, DateTime end)
        {
            DateTime currentDay = start.Date;
            while (currentDay <= end)
            {
                yield return currentDay;
                currentDay = currentDay.AddDays(1);
            }
        }
    }
}
