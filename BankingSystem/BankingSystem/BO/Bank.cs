using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class Bank
    {
        private List<Customer> customers;
        private int nextCustomerId;
        private int nextAccountNumber;

        public Bank()
        {
            customers = new List<Customer>();
            nextCustomerId = 1;
            nextAccountNumber = 1;
        }

        public BankAccount OpenAccount(string owner, string type, int balance)
        {
            var customer = customers.FirstOrDefault(c => c.Name == owner);
            if (customer == null)
            {
                customer = new Customer(nextCustomerId++, owner);
                customers.Add(customer);
            }

            BankAccount? account = null;

            if (type == "Savings")
            {
                account = new SavingsAccount(owner, balance, 1, 5000);
                account.AccountNumber = nextAccountNumber++;
            }
            else if (type == "Checking")
            {
                account = new CheckingAccount(owner, balance, 500);
                account.AccountNumber = nextAccountNumber++;
            }


            customer.Accounts.Add(account);

            return account;
        }

        public void CloseAccount(int accountNumber)
        {
            foreach (var customer in customers)
            {
                var account = customer.Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account != null)
                {
                    customer.Accounts.Remove(account);
                    return;
                }
            }

            throw new AccountNotFoundException();
        }

        public BankAccount FindAccount(int accountNumber)
        {
            return customers
                .SelectMany(c => c.Accounts)
                .FirstOrDefault(a => a.AccountNumber == accountNumber)
                ?? throw new AccountNotFoundException();
        }

        public List<BankAccount> ListAccounts()
        {
            return customers
                .SelectMany(c => c.Accounts)
                .ToList();
        }
    }
}