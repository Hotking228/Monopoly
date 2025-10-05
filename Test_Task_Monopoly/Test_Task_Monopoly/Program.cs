using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Task_Monopoly
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Попытка добавления транзакции разных типов");
            Console.WriteLine("---------------------------------------------------------");
            List<Wallet> w = Wallet.ReadAllWallets();
            Console.WriteLine("До добавления нового поступления " + w[0].ID + " " + w[0].GetCurrentBalance());
            Transaction transaction = new Transaction(Transaction.Count + 1, DateTime.Parse("2024-01-01"), 5000, TransactionType.Income, "Добавление нового поступления");
            w[0].TryTransaction(transaction);
            Console.WriteLine("после нового поступления " + w[0].ID + " " + w[0].GetCurrentBalance());

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("До добавления нового снятия " + w[1].ID + " " + w[1].GetCurrentBalance());
            transaction = new Transaction(Transaction.Count + 1, DateTime.Parse("2024-01-01"), 500, TransactionType.Expence, "Добавление нового снятия");
            w[1].TryTransaction(transaction);
            Console.WriteLine("после нового снятия " + w[1].ID + " " + w[1].GetCurrentBalance());

            Console.WriteLine("---------------------------------------------------------");

            Console.WriteLine();
            Console.WriteLine("Текущий баланс каждого кошелька");
            Console.WriteLine("---------------------------------------------------------");
            w = Wallet.ReadAllWallets();
            for (int i = 0; i < w.Count; i++)
            {
                Console.WriteLine(w[i].ID + " " + w[i].GetCurrentBalance());
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Месячные доходы каждого кошелька");
            Console.WriteLine("---------------------------------------------------------");
            w = Wallet.ReadAllWallets();
            for (int i = 0; i < w.Count; i++)
            {
                Console.WriteLine(w[i].ID + " " + w[i].GetMonthIncome(1, 2024));
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Месячные расходы каждого кошелька");
            Console.WriteLine("---------------------------------------------------------");
            w = Wallet.ReadAllWallets();
            for(int i = 0; i < w.Count; i++)
            {
                Console.WriteLine(w[i].ID + " " + w[i].GetMonthExpence(2,2024));
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Месячные транзакции, сгруппированные по типу и группы отсортированы по общей сумме трат и траты отсортированы по дате внутри групп");
            Console.WriteLine("---------------------------------------------------------");
            List<List<Transaction>> transactions = Transaction.GetMonthTransactions(1, 2024);
            for(int i = 0; i < transactions.Count; i++)
            {
                for(int j = 0; j < transactions[i].Count; j++)
                {
                    Console.WriteLine(transactions[i][j].ID + " " + transactions[i][j].Dt.ToString() + " " + transactions[i][j].Type);
                }
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("3 наибольште траты за месяц для каждого кошелька, отсортированные по убыванию траты");
            Console.WriteLine("---------------------------------------------------------");
            Dictionary<Wallet, List<int>> wallets = Wallet.GetThreeAllMostExpences(1, 2024);
            foreach(var wallet in wallets.Keys)
            {
                Console.WriteLine(wallet.ID + " " + string.Join(" ", wallets[wallet].Where(g => g != -1)));
            }
            Console.WriteLine("---------------------------------------------------------");
        }
    }
}
