using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    class Calculator
    {
        public Account Account { get; set; }
        public DateTime AccountDate { get; set; }
        public TransactionLog TxLog { get; set; }

        public double GetBalanceAtDate(DateTime date)
        {
            if (date < AccountDate)
            {
                throw new Exception($"Target date ({date}) must be greater than or equal to the account date ({AccountDate})");
            }

            double balance = Account.Balance;

            foreach (Transaction tx in TxLog.GetAllTransactionsBetween(AccountDate, date))
            {
                balance += (tx.Type == TransactionType.Credit ? tx.Amount : -tx.Amount);
            }

            return balance;
        }

        public SortedDictionary<DateTime, double> GetBalancesInRange(DateTime startDate, DateTime endDate, DateRangeType rangeType)
        {
            if (endDate < startDate)
            {
                throw new Exception($"Start date ({startDate}) must be less than or equal to end date ({endDate}).");
            }

            SortedDictionary<DateTime, double> balances = new SortedDictionary<DateTime, double>();

            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                balances.Add(currentDate, GetBalanceAtDate(currentDate));
                currentDate = GetNextDate(currentDate, rangeType);
            }

            return balances;
        }

        private DateTime GetNextDate(DateTime date, DateRangeType rangeType)
        {
            switch (rangeType)
            {
                case DateRangeType.Daily:
                    return date.AddDays(1);

                case DateRangeType.Weekly:
                    return date.AddDays(7);

                case DateRangeType.Monthly:
                    return date.AddMonths(1);

                case DateRangeType.Yearly:
                    return date.AddYears(1);

                default:
                    throw new Exception($"Unrecognized range type: {rangeType}");
            }
        }
    }
}
