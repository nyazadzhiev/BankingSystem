namespace BankingSystem.BO
{
    public class Bank
    {
        private Dictionary<int, BankAccount> accounts;
        private int uniqueId;

        public Bank()
        {
            uniqueId = 1;
            accounts = new Dictionary<int, BankAccount>();
        }

        public void OpenAccount(string owner, int balance)
        {
            accounts.Add(uniqueId++, new BankAccount(owner, balance));
        }

        public void CloseAccount(int accountNumber)
        {
            accounts.Remove(accountNumber);
        }

        public BankAccount FindAccount(int accountNumber)
        {
            return accounts[accountNumber];
        }

        public List<BankAccount> ListAccounts()
        {
            return accounts.Values.ToList();
        }
    }
}
