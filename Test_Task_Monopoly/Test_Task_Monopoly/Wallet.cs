using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Task_Monopoly
{
    public enum Currency
    {
        RUB,
        EU,
        USD
    }
    internal class Wallet
    {
        private HashSet<int> transactionsID;
        private int id;
        private string name;
        private string currency;
        private int startBalance;
        private static string fileName = "Wallets.txt";

        public int ID => id;
        public HashSet<int> TransactionID => transactionsID;
        public string Name => name;
        public string Currency => currency;
        public int StartBalance => startBalance;


        public Wallet(List<int> _transactionID, int _id, string _name, string _currency, int _startBalance)
        {
            transactionsID = new HashSet<int>();
            for(int i = 0; i < _transactionID.Count; i++)
            {
                transactionsID.Add(_transactionID[i]);
            }
            id = _id;
            name = _name;
            currency = _currency;
            startBalance = _startBalance;
        }

        private static Wallet ReadSingleWallet(StreamReader walletReader, Func<string[], bool> comparer = null)
        {
            string[] s = walletReader.ReadLine().Trim().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (comparer != null && !comparer(s) || s.Length == 0) return null;
            List<int> transactions = new List<int>();
            for (int i = 4; i < s.Length; i++)
            {
                transactions.Add(int.Parse(s[i]));
            }

            return new Wallet(transactions, int.Parse(s[0]), s[1], s[2], int.Parse(s[3]));
        }

        public static List<Wallet> ReadAllWallets(Func<string[], bool> comparer = null)
        {
            StreamReader walletReader = new StreamReader(fileName);
            List<Wallet> wallets = new List<Wallet>();
            while (!walletReader.EndOfStream)
            {
                Wallet w = ReadSingleWallet(walletReader, comparer);
                if(w != null)wallets.Add(w);
            }

            walletReader.Close();
            return wallets;
        }

        public int GetCurrentBalance()
        {
            List<Transaction> transactions = Transaction.ReadAllTransactions(s => transactionsID.Contains(int.Parse(s[0])));
            int currentBalance = startBalance;
            for(int i = 0; i < transactions.Count; i++)
            {
                currentBalance += transactions[i].Type == TransactionType.Income ? transactions[i].Sum : -transactions[i].Sum;
            }

            return currentBalance;
        }

        public bool TryTransaction(Transaction transaction)
        {
            int balance = GetCurrentBalance();
            if(transaction.Type == TransactionType.Expence)
            {
                if(balance >= transaction.Sum)
                {
                    Transaction.AddNewTransaction(transaction);
                    transactionsID.Add(transaction.ID);
                    ChangeTransactions(transaction.ID);
                    return true;
                }
                return false;
            }

            Transaction.AddNewTransaction(transaction);
            ChangeTransactions(transaction.ID);
            transactionsID.Add(transaction.ID);
            return true;
        }

        public int GetMonthIncome(int month, int year)
        {
            int income = 0;
            List<Transaction> transactions = Transaction.ReadAllTransactions(s => (TransactionType)int.Parse(s[3]) == TransactionType.Income 
                                                                              && transactionsID.Contains(int.Parse(s[0]))
                                                                              && year == DateTime.Parse(s[1]).Year
                                                                              && month == DateTime.Parse(s[1]).Month);
            for(int i = 0; i < transactions.Count; i++)
            {
                income += transactions[i].Sum;
            }

            return income;
        }

        private void ChangeTransactions(int newTransactionID)
        {
            StreamReader walletReader = new StreamReader(fileName);
            List<Wallet> wallets = ReadAllWallets();
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < wallets.Count; i++)
            {
                sb.Append(wallets[i].ID + ", " + wallets[i].Name + ", " + wallets[i].currency + ", " + wallets[i].StartBalance + ", " + string.Join(", ", wallets[i].transactionsID));
                if (wallets[i].ID == id)
                {
                    sb.Append(", " + newTransactionID);
                }
                sb.Append("\n");
            }

            walletReader.Close();
            StreamWriter walletWriter = new StreamWriter(fileName);
            walletWriter.WriteLine(sb.ToString());
            walletWriter.Close();
        }

        public int GetMonthExpence(int month, int year)
        {
            int expence = 0;
            List<Transaction> transactions = Transaction.ReadAllTransactions(s => (TransactionType)int.Parse(s[3]) == TransactionType.Expence
                                                                              && transactionsID.Contains(int.Parse(s[0]))
                                                                              && year == DateTime.Parse(s[1]).Year
                                                                              && month == DateTime.Parse(s[1]).Month);
            for (int i = 0; i < transactions.Count; i++)
            {
                expence += transactions[i].Sum;
            }

            return expence;
        }

        private void GetThreeSingleMostExpences(List<Transaction> transactions, List<int>ans)
        {
            List<Transaction> current = transactions.Where(t => transactionsID.Contains(t.ID)).ToList();
            ans.Add(-1);
            ans.Add(-1);
            ans.Add(-1);
            for (int i = 0; i < current.Count; i++)
            {
                if (current[i].Sum > ans[0])
                {
                    ans[2] = ans[1];
                    ans[1] = ans[0];
                    ans[0] = current[i].Sum;
                }
                else if (current[i].Sum > ans[1])
                {
                    ans[2] = ans[1];
                    ans[1] = current[i].Sum;
                }
                else if (current[i].Sum > ans[2])
                {
                    ans[2] = current[i].Sum;
                }
            }
        }

        public static Dictionary<Wallet, List<int>> GetThreeAllMostExpences(int month, int year)
        {
            StreamReader walletReader = new StreamReader(fileName);
            Dictionary<Wallet, List<int>> wallets = ReadAllWallets().ToDictionary(g => g, g => new List<int>());
            List<Transaction> transactions = Transaction.ReadAllTransactions(s =>
            {
                DateTime dt = DateTime.Parse(s[1]);
                return dt.Month == month && dt.Year == year && (TransactionType)int.Parse(s[3]) == TransactionType.Expence;
            });

            foreach(var w in wallets.Keys)
            {
                w.GetThreeSingleMostExpences(transactions, wallets[w]);
            }

            return wallets;
        }
    }
}
