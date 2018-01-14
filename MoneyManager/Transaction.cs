using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    class Transaction
    {
        public string Name { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public RecurringEvent Recurrence { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public Transaction ShallowCopyWithNewDate(DateTime newDate)
        {
            Transaction transaction = (Transaction)MemberwiseClone();
            transaction.Date = newDate;
            return transaction;
        }

        public Transaction ShallowCopyWithNoRecurrence()
        {
            Transaction transaction = (Transaction)MemberwiseClone();
            transaction.Recurrence = null;
            return transaction;
        }
    }
}
