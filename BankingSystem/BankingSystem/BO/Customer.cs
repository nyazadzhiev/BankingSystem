namespace BankingSystem.BO
{
    public class Customer
    {
        public int Id { get; }
        public string Name { get; }
        public List<BankAccount> Accounts { get; } = new();
        private readonly object _accountLock = new object();

        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddAccount(BankAccount account)
        {
            lock (_accountLock)
            {
                Accounts.Add(account);
            }
        }

        public bool RemoveAccount(BankAccount account)
        {
            lock (_accountLock)
            {
                return Accounts.Remove(account);
            }
        }
    }
}