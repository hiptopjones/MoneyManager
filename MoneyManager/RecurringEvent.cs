using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    abstract class RecurringEvent
    {
        public static RecurringEvent None = new NonRecurringEvent();

        public DateTime StartDate { get; set; }

        public DateTime EndDate
        {
            get
            {
                return StartDate.AddYears(2);
            }
        }

        public abstract DateTime? GetNextEvent(DateTime currentDate);

        public RecurringEvent ShallowCopyWithNewStartDate(DateTime newStartDate)
        {
            RecurringEvent recurrence = (RecurringEvent)MemberwiseClone();
            recurrence.StartDate = newStartDate;
            return recurrence;
        }
    }

    class NonRecurringEvent : RecurringEvent
    {
        public override DateTime? GetNextEvent(DateTime currentDate)
        {
            if (currentDate < StartDate)
            {
                return StartDate;
            }

            return null;
        }
    }

    class DailyRecurringEvent : RecurringEvent
    {
        public int Days { get; set; }

        public override DateTime? GetNextEvent(DateTime currentDate)
        {
            DateTime intervalDate = StartDate;

            while (intervalDate <= currentDate)
            {
                intervalDate += TimeSpan.FromDays(Days);
            }

            if (intervalDate <= EndDate)
            {
                return intervalDate;
            }

            return null;
        }
    }

    class WeeklyRecurringEvent : RecurringEvent
    {
        public int Weeks { get; set; }

        public override DateTime? GetNextEvent(DateTime currentDate)
        {
            DateTime intervalDate = StartDate;

            while (intervalDate <= currentDate)
            {
                intervalDate += TimeSpan.FromDays(Weeks * 7);
            }

            if (intervalDate <= EndDate)
            {
                return intervalDate;
            }

            return null;
        }
    }

    class MonthlyRecurringEvent : RecurringEvent
    {
        public int Months { get; set; }

        public override DateTime? GetNextEvent(DateTime currentDate)
        {
            DateTime intervalDate = StartDate;

            while (intervalDate <= currentDate)
            {
                intervalDate = intervalDate.AddMonths(Months);
            }

            if (intervalDate <= EndDate)
            {
                return intervalDate;
            }

            return null;
        }
    }

    class YearlyRecurringEvent : RecurringEvent
    {
        public int Years { get; set; }

        public override DateTime? GetNextEvent(DateTime currentDate)
        {
            DateTime intervalDate = StartDate;

            while (intervalDate <= currentDate)
            {
                intervalDate = intervalDate.AddYears(Years);
            }

            if (intervalDate <= EndDate)
            {
                return intervalDate;
            }

            return null;
        }
    }
}
