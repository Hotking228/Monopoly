using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Task_Monopoly
{
    public enum TransactionType
    {
        Expence,
        Income
    }

    internal class Transaction
    {
        private int id;
        private DateTime dt;
        private int sum;
        private TransactionType type;
        private string description;
        private static string fileName = "Transactions.txt";
        public int ID => id;
        public DateTime Dt => dt;
        public int Sum => sum;
        public TransactionType Type => type;
        public string Description => description;

        public static int Count
        {
            get { List<Transaction> transactions = ReadAllTransactions(); return transactions.Count; }
        }


        public Transaction(int _id,DateTime _dt, int _sum, TransactionType _type, string _desription)
        {
            description = _desription;
            id = _id;
            type = _type;
            id = _id;
            sum = _sum;
            dt = _dt;
            type = _type;
        }

        public static List<Transaction> ReadAllTransactions(Func<string[], bool> comparer = null)
        {
            string[] s;
            string str;
            List<Transaction> transactions = new List<Transaction>();
            StreamReader transactionsReader = new StreamReader(fileName);
            while ((str = transactionsReader.ReadLine()) != null)
            {
                s = str.Trim().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if((comparer == null || comparer(s))) transactions.Add (new Transaction(int.Parse(s[0]), DateTime.Parse(s[1]), int.Parse(s[2]), (TransactionType)int.Parse(s[3]), s[4]));
            }
            
            transactionsReader.Close();
            return transactions;
        }

        public static List<List<Transaction>> GetMonthTransactions(int month, int year)
        {
            List<Transaction> transactions = ReadAllTransactions(s =>
            {
                DateTime dt = DateTime.Parse(s[1]);
                return dt.Month == month && dt.Year == year;
            });
            List<List<Transaction>> ans = transactions
                                .GroupBy(transaction => transaction.Type)
                                .Select(group => new
                                {
                                    Type = group.Key,
                                    Transactions = group.OrderByDescending(t => t.Dt).ToList(),
                                    TotalSum = group.Sum(t => t.Sum)
                                })
                                .OrderBy(group => group.TotalSum)
                                .Select(group => group.Transactions)
                                .ToList();
            return ans;
        }

        public static void AddNewTransaction(Transaction transaction)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            StreamWriter transactionWriter = fileInfo.AppendText();
           // transactionWriter.WriteLine();
            transactionWriter.WriteLine(transaction.ID + ", " + transaction.Dt.Year + "-" + transaction.Dt.Month + "-" + transaction.Dt.Day + ", " + transaction.Sum + ", " + (int)transaction.Type + ", " + transaction.description);
            transactionWriter.Close();
        }
    }
}
