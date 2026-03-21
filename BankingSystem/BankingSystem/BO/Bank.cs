using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class Bank
    {
        private List<Customer> _customers;
        private int _nextCustomerId;
        private int _nextAccountNumber;
        private readonly object _bankLock = new object();
        public decimal WithdrawalFee { get; set; } = 0.50m;

        public Bank()
        {
            _customers = new List<Customer>();
            _nextCustomerId = 1;
            _nextAccountNumber = 1;
        }

        public BankAccount OpenAccount(string owner, string type, decimal initialBalance, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner name cannot be empty", nameof(owner));

            if (initialBalance < 0)
                throw new InvalidAmountException();

            lock (_bankLock)
            {
                var customer = _customers.FirstOrDefault(c => c.Name == owner);
                if (customer == null)
                {
                    customer = new Customer(_nextCustomerId++, owner);
                    _customers.Add(customer);
                }

                BankAccount account = type.ToLower() switch
                {
                    "savings" when parameters.Length >= 2 =>
                        new SavingsAccount(owner, customer.Id, initialBalance,
                            Convert.ToDecimal(parameters[0]), Convert.ToDecimal(parameters[1])),

                    "checking" when parameters.Length >= 1 =>
                        new CheckingAccount(owner, customer.Id, initialBalance,
                            Convert.ToDecimal(parameters[0])),

                    _ => throw new ArgumentException($"Invalid account type '{type}' or missing parameters")
                };

                account.AccountNumber = _nextAccountNumber++;
                customer.Accounts.Add(account);

                return account;
            }
        }

        public void CloseAccount(int accountNumber)
        {
            lock (_bankLock)
            {
                foreach (var customer in _customers)
                {
                    var account = customer.Accounts
                        .FirstOrDefault(a => a.AccountNumber == accountNumber);

                    if (account != null)
                    {
                        if (account.GetBalance() != 0)
                            throw new InvalidOperationException("Cannot close account with non-zero balance");

                        customer.Accounts.Remove(account);
                        return;
                    }
                }

                throw new AccountNotFoundException();
            }
        }

        public BankAccount FindAccount(int accountNumber)
        {
            lock (_bankLock)
            {
                var account = _customers
                    .SelectMany(c => c.Accounts)
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account == null)
                    throw new AccountNotFoundException();

                return account;
            }
        }

        public List<BankAccount> ListAccounts()
        {
            lock (_bankLock)
            {
                return _customers
                    .SelectMany(c => c.Accounts)
                    .ToList();
            }
        }

        public IEnumerable<Transaction> GenerateStatement(int accountNumber, DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentException("From date must be before to date");

            var account = FindAccount(accountNumber);

            lock (account)
            {
                return account.Transactions
                    .Where(t => t.Timestamp >= from && t.Timestamp <= to)
                    .OrderBy(t => t.Timestamp)
                    .ToList();
            }
        }

        public Customer FindCustomer(int customerId)
        {
            lock (_bankLock)
            {
                return _customers.FirstOrDefault(c => c.Id == customerId)
                    ?? throw new ArgumentException("Customer not found");
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            lock (_bankLock)
            {
                return _customers.ToList();
            }
        }
    }
}