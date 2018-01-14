using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    class Program
    {
        private const int SuccessExitCode = 0;
        private const int FailureExitCode = 1;

        static int Main(string[] args)
        {
            // Default to success
            int exitCode = SuccessExitCode;

            CommandLineOptions options = new CommandLineOptions();

            try
            {
                if (!options.ParseArguments(args))
                {
                    exitCode = FailureExitCode;
                }
                else
                {
                    ProcessTransactions(options);
                    exitCode = SuccessExitCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Processing failed: {ex.Message}");
            }
            finally
            {
                if (options.PromptUser || Debugger.IsAttached)
                {
                    PromptToContinue();
                }
            }

            return exitCode;
        }

        static void ProcessTransactions(CommandLineOptions options)
        {
            TransactionLog txLog = LoadTransactionLog(options.TransactionLogFilePath);

            Calculator calculator = new Calculator
            {
                Account = new Account { Balance = options.AccountBalance },
                AccountDate = options.AccountBalanceDate.Date,
                TxLog = txLog,
            };

            List<string> labels = new List<string>();
            List<double> values = new List<double>();

            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddMonths(12);

            SortedDictionary<DateTime, double> balances = calculator.GetBalancesInRange(startDate, endDate, DateRangeType.Daily);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            foreach (DateTime date in balances.Keys)
            {
                double totalMillis = date.ToUniversalTime().Subtract(unixEpoch).TotalMilliseconds;
                labels.Add($"new Date({totalMillis})");
                values.Add(balances[date]);
            }

            string chartHtml = LoadChartHtml();
            string labelJsArray = "[" + string.Join(", ", labels) + "]";
            string valuesJsArray = "[" + string.Join(",", values) + "]";

            chartHtml = chartHtml.Replace("{LabelsArray}", labelJsArray);
            chartHtml = chartHtml.Replace("{ValuesArray}", valuesJsArray);

            string chartHtmlFilePath = @"c:\users\peterj\desktop\chart.html";
            File.WriteAllText(chartHtmlFilePath, chartHtml);
            Process.Start(chartHtmlFilePath);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Hit a key to continue...");
                Console.ReadKey();
            }
        }

        private static string LoadChartHtml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("MoneyManager.HTML.Chart.html")))
            {
                return reader.ReadToEnd();
            }
        }
        private static TransactionLog LoadTransactionLog(string txLogFilePath)
        {
            TransactionLog txLog;

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

            if (File.Exists(txLogFilePath))
            {
                Console.WriteLine("Loading transaction log...");
                string json = File.ReadAllText(txLogFilePath);
                txLog = JsonConvert.DeserializeObject<TransactionLog>(json, jsonSettings);
            }
            else
            {
                Console.WriteLine("Generating transaction log...");
                txLog = GenerateTransactionLog(txLogFilePath, jsonSettings);
            }

            return txLog;
        }

        private static TransactionLog GenerateTransactionLog(string filePath, JsonSerializerSettings jsonSettings)
        {
            Console.WriteLine("Generating transaction log...");
            TransactionLog txLog = new TransactionLog();

            RecurringEvent biWeeklyRecurrence = new WeeklyRecurringEvent
            {
                Weeks = 2
            };

            RecurringEvent monthlyRecurrence = new MonthlyRecurringEvent
            {
                Months = 1
            };

            txLog.Transactions.Add(
                new Transaction
                {
                    Name = "Salary",
                    Type = TransactionType.Credit,
                    Amount = 100,
                    Date = new DateTime(2017, 1, 1),
                    Recurrence = biWeeklyRecurrence.ShallowCopyWithNewStartDate(new DateTime(2017, 1, 1))
                }
            );

            txLog.Transactions.Add(
                new Transaction
                {
                    Name = "Mortgage",
                    Type = TransactionType.Debit,
                    Amount = 150,
                    Date = new DateTime(2017, 1, 1),
                    Recurrence = biWeeklyRecurrence.ShallowCopyWithNewStartDate(new DateTime(2017, 1, 1))
                }
            );

            txLog.Transactions.Add(
                new Transaction
                {
                    Name = "Strata Fees",
                    Type = TransactionType.Debit,
                    Amount = 20,
                    Date = new DateTime(2017, 1, 1),
                    Recurrence = monthlyRecurrence.ShallowCopyWithNewStartDate(new DateTime(2017, 1, 1))
                }
            );

            // Persist the transaction log
            string json = JsonConvert.SerializeObject(txLog, Formatting.Indented, jsonSettings);
            File.WriteAllText(filePath, json);

            return txLog;
        }

        private static void PromptToContinue()
        {
            Console.WriteLine("Hit a key to continue...");
            Console.ReadKey();
        }
    }
}
