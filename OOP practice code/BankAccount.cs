using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_practice_code
{
    public class BankAccount
    {
        #region declare the attributes/ properties
        public string Number { get; }
        public string Owner { get; set; }
        public decimal Balence
        {
            get
            {
                decimal balence = 0;
                foreach (var item in allTransactions)
                {
                    balence += item.Amount;
                }
                return balence;
            } 
        }

        private static int accountNumberSeed = 1234567890;
        private List<Transaction> allTransactions = new List<Transaction>();
        #endregion

        #region declare methods
        //counstructor method
        public BankAccount(string name, decimal initialBalence)
        {
            this.Owner = name;
            MakeDeposit(initialBalence, DateTime.Now, "Initial Balence");
            this.Number = accountNumberSeed.ToString();
            //increment accountNumberSeed by 1
            accountNumberSeed++;
        }

        public void MakeDeposit(decimal amount, DateTime date, string note)
        {
            if (amount <= 0)
            {
                //throws stops the running of the program once run
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
            }
            var deposit = new Transaction(amount, date, note);
            allTransactions.Add(deposit);
        }

        public void makeWithdrawal(decimal amount, DateTime date, string note)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount of withdrawal must be positive");
            }
            if (Balence - amount < 0)
            {
                throw new InvalidOperationException("You're too broke for that. Go to work.");
            }
            var withdrawal = new Transaction(-amount, date, note);
            allTransactions.Add(withdrawal);
        }

        public string GetAccountHistory()
        {
            var report = new StringBuilder();
            report.AppendLine("Date \t\t Amount \t\t Note");
            foreach (var item in allTransactions)
            {
                //ROWS
                report.AppendLine($"{item.Date.ToShortDateString()}\t £{item.Amount}\t{item.Notes}");
            }
            return report.ToString();
        }
        #endregion
    }
}
