using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Transactions;

public class Transaction
{
    public string Date { get; set; }
    public string TxnId { get; set; }
    public string Account { get; set; }
    public string Type { get; set; }
    public double Amount { get; set; }
}

public class InterestRule
{
    public DateTime Date { get; }
    public string RuleId { get; }
    public double Rate { get; }

    public InterestRule(DateTime date, string ruleId, double rate)
    {
        Date = date;
        RuleId = ruleId;
        Rate = rate;
    }
}

public class BankingSystem
{
    public Dictionary<string, Dictionary<string, object>> accounts;
    public List<Transaction> transactions;
    public List<InterestRule> interestRules;

    public BankingSystem()
    {
        accounts = new Dictionary<string, Dictionary<string, object>>();
        transactions = new List<Transaction>();
        interestRules = new List<InterestRule>();
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
            Console.WriteLine("[I]nput transactions");
            Console.WriteLine("[D]efine interest rules");
            Console.WriteLine("[P]rint statement");
            Console.WriteLine("[Q]uit");

            string choice = Console.ReadLine().ToUpper();

            switch (choice)
            {
                case "I":
                    InputTransactions();
                    break;
                case "D":
                    DefineInterestRule();
                    break;
                case "P":
                    PrintStatement();
                    break;
                case "Q":
                    Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                    Console.WriteLine("Have a nice day!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }
    }

    public void InputTransactions()
    {
        while (true)
        {
            Console.WriteLine("Please enter transaction details in <Date>|<Account>|<Type>|<Amount> format");
            Console.WriteLine("(or enter blank to go back to the main menu):");
            Console.Write("> ");
            string input = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(input))
                return;

            ProcessTransaction(input);
        }
    }

    public void ProcessTransaction(string input)
    {
        string[] transactionParts = input.Split('|');
        if (transactionParts.Length != 4)
        {
            Console.WriteLine("Invalid transaction format. Please try again.");
            return;
        }

        if (!DateTime.TryParseExact(transactionParts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dates))
        {
            Console.WriteLine("Invalid date format. Please use YYYYMMdd.");
            return;
        }

        string transactionType = transactionParts[2].ToUpper();
        if (transactionType != "D" && transactionType != "W")
        {
            Console.WriteLine("Invalid transaction type. Use 'D' for deposit or 'W' for withdrawal.");
            return;
        }

        if (!decimal.TryParse(transactionParts[3], out decimal amounts) || amounts <= 0)
        {
            Console.WriteLine("Invalid amount. Please enter a positive number.");
            return;
        }



        string[] parts = input.Split('|');
        string date = parts[0].Trim();
        string account = parts[1].Trim();
        string type = parts[2].Trim().ToUpper();
        double amount = double.Parse(parts[3].Trim());

        if (!accounts.ContainsKey(account))
            accounts[account] = new Dictionary<string, object> { { "balance", 0.0 }, { "transactions", new List<Transaction>() } };

        if (type == "W" && (double)accounts[account]["balance"] < amount)
        {
            Console.WriteLine("Insufficient balance. Withdrawal canceled.");
            return;
        }

        string txnId = $"{date}-{((List<Transaction>)accounts[account]["transactions"]).Count + 1:00}";

        if (type == "D")
            accounts[account]["balance"] = (double)accounts[account]["balance"] + amount;
        else
            accounts[account]["balance"] = (double)accounts[account]["balance"] - amount;

        Transaction transaction = new Transaction { Date = date, TxnId = txnId, Account = account, Type = type, Amount = amount };
        transactions.Add(transaction);
        ((List<Transaction>)accounts[account]["transactions"]).Add(transaction);

        PrintAccountStatement(account);
    }

    public void DefineInterestRule()
    {
        Console.WriteLine("Please enter interest rules details in <Date>|<RuleId>|<Rate in %> format");
        Console.WriteLine("(or enter blank to go back to the main menu):");
        Console.Write("> ");
        string input = Console.ReadLine().Trim();

        if (string.IsNullOrWhiteSpace(input))
            return;

        ProcessInterestRule(input);
    }

    public void ProcessInterestRule(string input)
    {
        string[] parts = input.Split('|');

        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid interest rule format. Please try again.");
            return;
        }

        if (!DateTime.TryParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dates))
        {
            Console.WriteLine("Invalid date format. Please use YYYYMMdd.");
            return;
        }

        if (!double.TryParse(parts[2], out double rate) || rate <= 0 || rate >= 100)
        {
            Console.WriteLine("Invalid interest rate. Please enter a rate between 0 and 100.");
            return;
        }

        string ruleId = parts[1].Trim();

        interestRules.RemoveAll(r => r.Date == dates);
        interestRules.Add(new InterestRule(dates, ruleId, rate));

        PrintInterestRules();
    }

    public void PrintAccountStatement(string account)
    {
        Console.WriteLine($"Account: {account}");
        Console.WriteLine("Date     | Txn Id      | Type | Amount | Balance");
        double balance = 0;

        foreach (Transaction txn in ((List<Transaction>)accounts[account]["transactions"]))
        {
            balance += txn.Type == "D" ? txn.Amount : -txn.Amount;
            Console.WriteLine($"{txn.Date} | {txn.TxnId} | {txn.Type}    | {txn.Amount:F2} | {balance:F2} |");
        }
    }

    public void PrintInterestRules()
    {
        Console.WriteLine("Interest rules:");
        Console.WriteLine("Date     | RuleId | Rate (%) |");

        foreach (InterestRule rule in interestRules.OrderBy(r => r.Date))
        {
            Console.WriteLine($"{rule.Date.ToString("yyyyMMdd")} | {rule.RuleId} | {rule.Rate:F2} |");
        }
    }

    public void PrintStatement()
    {
        Console.WriteLine("Please enter account and month to generate the statement <Account>|<Month>");
        Console.WriteLine("(or enter blank to go back to the main menu):");
        Console.Write("> ");
        string input = Console.ReadLine().Trim();

        if (string.IsNullOrWhiteSpace(input))
            return;

        string[] statementParts = input.Split('|');
        if (statementParts.Length != 2)
        {
            Console.WriteLine("Invalid statement format. Please try again.");
            return;
        }

        string accountNumber = statementParts[0];
        if (!accounts.ContainsKey(accountNumber))
        {
            Console.WriteLine("Account not found. Please enter a valid account number.");
            return;
        }

        if (!int.TryParse(statementParts[1], out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month. Please enter a valid month (1-12).");
            return;
        }

        string[] parts = input.Split('|');
        string account = parts[0].Trim();
       

        PrintAccountStatement(account);
        CalculateAndDisplayInterest(account, month);
    }

    public void CalculateAndDisplayInterest(string account, int month=0)
    {
        double interestAccumulated = 0;
        double dailyInterestRate = 0;
        InterestRule applicableRule = null;

        var transaction = transactions.OrderBy(t => t.Date).ToList();

        if (transaction.Count == 0)
            return;

        var lastTransactionDate = transaction.Last().Date;
        var lastDayOfMonth = new DateTime(int.Parse(lastTransactionDate.Substring(0, 4)), int.Parse(lastTransactionDate.Substring(4, 2)), DateTime.DaysInMonth(int.Parse(lastTransactionDate.Substring(0, 4)), int.Parse(lastTransactionDate.Substring(4, 2))));

        if (DateTime.Now.Date != lastDayOfMonth)
            return; 



        foreach (InterestRule rule in interestRules.OrderBy(r => r.Date))
        {
            if (rule.Date.Month == month || month == 0)
            {
                applicableRule = rule;
            }
        }

        if (applicableRule!= null)
        {
            double balance = (double)accounts[account]["balance"];
            dailyInterestRate = applicableRule.Rate / 100 / DateTime.DaysInMonth(DateTime.Now.Year, month);
            interestAccumulated = balance * dailyInterestRate * DateTime.DaysInMonth(DateTime.Now.Year, month);
            balance += interestAccumulated;
            Console.WriteLine($"{lastDayOfMonth.ToString("yyyyMMdd"):D2} |               |      I       |   {interestAccumulated:F2}        | {balance + dailyInterestRate:F2} |");

        }

        string interestTxnId = $"{lastTransactionDate}-I";
        Transaction interestTransaction = new Transaction
        {
            Date = lastTransactionDate,
            TxnId = interestTxnId,
            Account = account,
            Type = "I",
            Amount = interestAccumulated
        };

        transactions.Add(interestTransaction);
    }
}

class Program
{
    static void Main(string[] args)
    {
        BankingSystem bank = new BankingSystem();
        bank.Run();
    }
}



