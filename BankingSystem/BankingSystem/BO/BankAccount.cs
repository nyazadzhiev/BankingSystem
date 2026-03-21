using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public abstract class BankAccount
    {
        protected string _owner;
        private decimal _balance = 0;
        public List<Transaction> Transactions { get; set; }
        protected decimal _dailyWithdrawalLimit;
        protected decimal _withdrawnToday;
        protected DateTime _lastWithdrawnDate;
        public int AccountNumber { get; set; }
        public int CustomerId { get; set; }

        // Thread safety lock object
        protected readonly object _balanceLock = new object();

        public BankAccount(string owner, int customerId, decimal balance)
        {
            _owner = owner;
            CustomerId = customerId;
            _balance = balance;
            Transactions = new List<Transaction>();
            _dailyWithdrawalLimit = 1000000;
            _withdrawnToday = 0;
            _lastWithdrawnDate = DateTime.UtcNow.Date;
        }

        public decimal GetBalance()
        {
            lock (_balanceLock)
            {
                return _balance;
            }
        }

        protected void SetBalance(decimal balance)
        {
            lock (_balanceLock)
            {
                if (balance < 0 && !CanHaveNegativeBalance())
                    throw new InsufficientFundsException();

                _balance = balance;
            }
        }

        protected virtual bool CanHaveNegativeBalance() => false;

        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException();

            lock (_balanceLock)
            {
                _balance += amount;
                LogTransaction(TransactionType.Deposit, amount);
            }
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException();

            lock (_balanceLock)
            {
                ResetDailyLimit();

                if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                    throw new DailyLimitExceededException();

                if (_balance - amount < 0 && !CanHaveNegativeBalance())
                    throw new InsufficientFundsException();

                _balance -= amount;
                _withdrawnToday += amount;
                _lastWithdrawnDate = DateTime.UtcNow;

                LogTransaction(TransactionType.Withdraw, amount);
            }
        }

        public virtual void TransferTo(BankAccount account, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException();

            lock (_balanceLock)
            {
                lock (account._balanceLock)
                {
                    if (!CanHaveNegativeBalance() && _balance - amount < 0)
                        throw new InsufficientFundsException();

                    ResetDailyLimit();
                    if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                        throw new DailyLimitExceededException();

                    _balance -= amount;
                    _withdrawnToday += amount;
                    _lastWithdrawnDate = DateTime.UtcNow;

                    account._balance += amount;

                    LogTransaction(TransactionType.Transfer, amount, account.AccountNumber);
                    account.LogTransaction(TransactionType.Transfer, amount, null, AccountNumber);
                }
            }
        }

        protected virtual void ResetDailyLimit()
        {
            DateTime today = DateTime.UtcNow.Date;
            if (_lastWithdrawnDate.Date != today)
            {
                _withdrawnToday = 0;
                _lastWithdrawnDate = today;
            }
        }

        protected void LogTransaction(TransactionType type, decimal amount, int? toAccount = null, int? fromAccount = null)
        {
            lock (_balanceLock)
            {
                Transactions.Add(new Transaction
                {
                    Type = type,
                    Amount = amount,
                    FromAccount = fromAccount ?? (type == TransactionType.Withdraw ? AccountNumber : (int?)null),
                    ToAccount = toAccount ?? (type == TransactionType.Deposit ? AccountNumber : (int?)null),
                    ResultingBalance = _balance
                });
            }
        }
    }
}