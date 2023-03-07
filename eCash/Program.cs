namespace eCash
{
    internal class Program
    {
        private static readonly AccountManager AccountManager = new();
        private static readonly DetailManager DetailManager = new();
        
        static void Main(string[] args)
        {
            Account account = Login();
            Service(account);
        }

        public static Account Login()
        {
            Console.WriteLine("Welcome to eCash!");
            Console.WriteLine("Please enter your username:");
            string userName = Utility.ReadUserName();
            if (AccountManager.IsAccountExist(userName))
            {
                return AccountManager.GetAccount(userName);
            }
            else
            {
                
                AccountManager.SignUp(userName);
                return AccountManager.GetAccount(userName);
            }
        }
        
        
        public static void Service(Account account)
        {
            bool isContinued = true;
            while (isContinued)
            {
                Console.WriteLine("Welcome to eCash!");
                
                Console.WriteLine("Please select an option:");
                Console.WriteLine("s) 儲值");
                Console.WriteLine("p) 消費");
                Console.WriteLine("d) 查詢餘額");
                Console.WriteLine("q) Quit");
                char c = Utility.ReadMenuSelection();
                int amount = 0;
                bool res = true;
                switch (c)
                {
                    case 's':
                        amount = Utility.ReadNum();
                        account.Deposite(amount);
                        Console.WriteLine($"您存了{amount}元，帳戶目前額逾: {account.Balance}元");
                        break;
                    
                    case 'p':
                        amount = Utility.ReadNum();
                        res = account.Withdraw(amount);
                        if (res)
                        {
                            Console.WriteLine($"您消費{amount}元，帳戶目前額逾: {account.Balance}元");   
                        }
                        else
                        {
                            Console.WriteLine($"您的帳戶餘額不足，請重新輸入: ");
                        }
                        break;
                    case 'd':
                        var accountBalance = account.Balance;
                        Console.WriteLine($"您的帳戶目前餘額{accountBalance}");
                        break;
                    case 'q':
                        Console.WriteLine($"請您再次確定是否離開(Y/N): ");
                        res = Utility.ReadConfirmSelection();
                        isContinued = !res;
                        break;
                }
            } 
        }
    }

    public class Account
    {
        public string UserName { get; init; }
        public int Balance { get; private set; }

        public Account(string userName)
        {
            UserName = userName;
            Balance = 0;
        }

        public Account(string userName, int balance)
        {
            UserName = userName;
            Balance = balance;
        }

        public bool Deposite(int money)
        {
            if (money >= 0)
            {
                Balance += money;
                return true;
            }

            return false;
        }

        public bool Withdraw(int money)
        {
            if (money >= 0 && Balance >= money)
            {
                Balance -= money;
                return true;
            }

            return false;            
        }
        
        public bool Transfer(Account account, int money)
        {
            if (Withdraw(money))
            {
                account.Deposite(money);
                return true;
            }

            return false;
        }
        
        public override string ToString()
        {
            return $"UserName: {UserName}, Balance: {Balance}";
        }
        
    }
    
    public class AccountManager
    {
        private readonly IDictionary<string, Account> _accounts = new Dictionary<string, Account>();
        
        public void AddAccount(Account account)
        {
            _accounts.Add(account.UserName, account);
        }
        
        public void RemoveAccount(Account account)
        {
            _accounts.Remove(account.UserName);
        }
        
        public Account GetAccount(string userName)
        {
            return _accounts[userName];
        }
        
        public void PrintAllAccounts()
        {
            foreach (var account in _accounts)
            {
                Console.WriteLine(account);
            }
        }
        public bool IsAccountExist(string userName)
        {
            return _accounts.ContainsKey(userName);
        }
        
        public void SignUp(string userName)
        {
            if (!IsAccountExist(userName))
            {
                var account = new Account(userName);
                AddAccount(account);
            }
        }
    }

    public class Detail
    {
        public string UserName { get; init; }
        TransactionType TransactionType { get; init; }
        public int Money { get; init; }
        public DateTime Time { get; init; }
        
        public Detail(string userName, TransactionType transactionType, int money, DateTime time)
        {
            UserName = userName;
            TransactionType = transactionType;
            Money = money;
            Time = time;
        }
        
        public override string ToString()
        {
            return $"UserName: {UserName}, TransactionType: {TransactionType}, Money: {Money}, Time: {Time}";
        }
    }

    public class DetailManager
    {
        private readonly IDictionary<string, List<Detail>> _details = new Dictionary<string, List<Detail>>();
        public void AddDetail(Detail detail)
        {
            if (_details.ContainsKey(detail.UserName))
            {
                _details[detail.UserName].Add(detail);
            }
            else
            {
                _details.Add(detail.UserName, new List<Detail> {detail});
            }
        }
        
        public List<Detail> GetDetails(string userName)
        {
            return _details[userName];
        }

    }
    
    public enum TransactionType
    {
        Deposite,
        Withdraw,
        Transfer
    }   

    public static class Utility
    {
        public static char ReadMenuSelection()
        {
            char c;
            while (true)
            {
                string? str = Console.ReadLine();
                var canadidate = new char[] {'s', 'p', 'd', 'q'};
                if (str != null && str.Length == 1)
                {
                    c = str[0];
                    if (canadidate.Contains(c))
                    {
                        return c;
                    }
                }

                Console.WriteLine("Invalid selection, please try again.");
            }
        }

        public static int ReadNum()
        {
            while (true)
            {
                string? str = Console.ReadLine();
                if (str != null && int.TryParse(str, out int num))
                {
                    return num;
                }

                Console.WriteLine("Invalid input, please try again.");
            }
        }

        public static bool ReadConfirmSelection()
        {
            char c;
            while (true)
            {
                string? str = Console.ReadLine();
                var canadidate = new char[] {'y', 'n'};
                if (str != null && str.Length == 1)
                {
                    c = str.ToLower()[0];
                    if (canadidate.Contains(c))
                    {
                        return c == 'y';
                    }
                }

                Console.WriteLine("請輸入 'y' 或者是 'n'.");
            }
        }

        private static String ReadKeyBoard(int limit)
        {
            string? input = "";
            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || input.Length > limit)
                {
                    Console.WriteLine("Invalid input, please try again.");
                    continue;
                }

                break;
            }

            return input;
        }
        
        public static String ReadUserName()
        {
            return ReadKeyBoard(255);
        }
        
    }
}