namespace BankingSystem
{
    public class BankAccount
    {
        protected string _owner;
        private int _balance = 0;
        private List<Transaction> _transactions;

        public BankAccount(string owner, int balance)
        {
            _owner = owner;
            _balance = balance;
            _transactions = new List<Transaction>();
        }

        public int GetBalance()
        {
            return _balance;
        }

        public void SetBalance(int balance)
        {
            if (balance < 0)
                throw new Exception(" invalid input");

            _balance = balance;
        }

        public void Deposit(int amount)
        {
            if(amount <= 0)
                throw new Exception(" invalid input");

            _balance += amount;

            LogTransaction(TransactionType.Deposit, amount);
        }

        public virtual void Withdraw(int amount)
        {
            if (amount > this._balance)
                throw new Exception(" insufficient funds");

            _balance -= amount;

            LogTransaction(TransactionType.Withdraw, amount);
        }

        public void TransferTo(BankAccount account, int amount)
        { 
            Withdraw(amount);
            account.Deposit(amount);

            LogTransaction(TransactionType.Transfer, amount);
        }

        protected void LogTransaction(TransactionType type, decimal amount, int? fromAccount = null, int? toAccount = null)
        {
            _transactions.Add(new Transaction
            {
                Type = type,
                Amount = amount,
                FromAccount = fromAccount,
                ToAccount = toAccount,
                ResultingBalance = _balance
            });
        }
    }
}
