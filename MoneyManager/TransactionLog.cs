using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    class TransactionLog
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        private SortedDictionary<DateTime, List<Transaction>> TransactionsMap { get; set; } = new SortedDictionary<DateTime, List<Transaction>>();
        
        public List<Transaction> GetAllTransactions()
        {
            return GetAllTransactionsBetween(DateTime.MinValue, DateTime.MaxValue);
        }

        public List<Transaction> GetAllTransactionsBetween(DateTime startDate, DateTime endDate)
        {
            EnsureTransactionsAreFlattened();

            List<Transaction> allTransactions = new List<Transaction>();

            IEnumerable<DateTime> dateRange = TransactionsMap.Keys.Where(k => k > startDate && k < endDate);
            foreach (DateTime date in dateRange)
            {
                allTransactions.AddRange(TransactionsMap[date]);
            }

            return allTransactions;
        }

        private void EnsureTransactionsAreFlattened()
        {
            if (!TransactionsMap.Any())
            {
                FlattenTransactions();
            }
        }

        private void FlattenTransactions()
        {
            foreach (Transaction tx in Transactions)
            {
                FlattenTransaction(tx);
            }
        }

        public void FlattenTransaction(Transaction transaction)
        {
            const int MaxAgeInYears = 5;
            if (transaction.Date < DateTime.Today.AddYears(-MaxAgeInYears))
            {
                throw new Exception($"Transaction {transaction.Name} for ${transaction.Amount} occurred more than {MaxAgeInYears} years ago.");
            }

            // Strip the recurrence out of the transaction, so that all flattened
            // transactions are one-time transactions
            RecurringEvent recurrence = transaction.Recurrence;
            transaction = transaction.ShallowCopyWithNoRecurrence();

            List<Transaction> transactions = GetTransactionListForDate(transaction.Date);
            transactions.Add(transaction);

            if (recurrence != null)
            {
                DateTime? nextDate = recurrence.GetNextEvent(transaction.Date);
                while (nextDate != null)
                {
                    transaction = transaction.ShallowCopyWithNewDate((DateTime)nextDate);

                    transactions = GetTransactionListForDate(transaction.Date);
                    transactions.Add(transaction);

                    nextDate = recurrence.GetNextEvent(transaction.Date);
                }
            }
        }

        private List<Transaction> GetTransactionListForDate(DateTime dateTime)
        {
            List<Transaction> transactions;
            if (!TransactionsMap.TryGetValue(dateTime, out transactions))
            {
                transactions = new List<Transaction>();
                TransactionsMap.Add(dateTime, transactions);
            }

            return transactions;
        }
    }
}
