using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class CheckingAccount : BankAccount
    {
        private decimal _overdraftLimit;
        private bool _overdraftFeeChargedToday = false;
        private const decimal OVERDRAFT_FEE = 35.00m;
        private readonly object _feeLock = new object();

        public CheckingAccount(string owner, int customerId, decimal balance, decimal overdraftLimit)
            : base(owner, customerId, balance)
        {
            _overdraftLimit = overdraftLimit;
        }

        protected override bool CanHaveNegativeBalance() => true;

        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException();

            lock (_balanceLock)
            {
                ResetDailyLimit();

                if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                    throw new DailyLimitExceededException();

                decimal currentBalance = GetBalance();
                decimal newBalance = currentBalance - amount;

                if (newBalance < -_overdraftLimit)
                    throw new OverdraftLimitExceededException();

                SetBalance(newBalance);
                _withdrawnToday += amount;
                _lastWithdrawnDate = DateTime.UtcNow;

                LogTransaction(TransactionType.Withdraw, amount);
                CheckAndApplyOverdraftFee();
            }
        }

        private void CheckAndApplyOverdraftFee()
        {
            lock (_feeLock)
            {
                if (GetBalance() < 0 && !_overdraftFeeChargedToday)
                {
                    SetBalance(GetBalance() - OVERDRAFT_FEE);
                    LogTransaction(TransactionType.Fee, OVERDRAFT_FEE);
                    _overdraftFeeChargedToday = true;
                }

                if (DateTime.UtcNow.Date != _lastWithdrawnDate.Date)
                {
                    _overdraftFeeChargedToday = false;
                }
            }
        }

        public void ResetOverdraftFeeFlag()
        {
            lock (_feeLock)
            {
                _overdraftFeeChargedToday = false;
            }
        }
    }
}