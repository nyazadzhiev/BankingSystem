using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class SavingsAccount : BankAccount
    {
        private decimal _interestRatePercent;
        private decimal _maxBalance;
        private readonly object _interestLock = new object();

        public SavingsAccount(string owner, int customerId, decimal balance, decimal interestRate, decimal maxBalance)
            : base(owner, customerId, balance)
        {
            _interestRatePercent = interestRate;
            _maxBalance = maxBalance;
        }

        protected override bool CanHaveNegativeBalance() => false;

        public override void Withdraw(decimal amount)
        {
            lock (_balanceLock)
            {
                ResetDailyLimit();

                if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                    throw new DailyLimitExceededException();

                decimal currentBalance = GetBalance();
                if (currentBalance - amount < 0)
                    throw new InsufficientFundsException();

                base.Withdraw(amount);
            }
        }

        public override void Deposit(decimal amount)
        {
            lock (_balanceLock)
            {
                decimal currentBalance = GetBalance();
                if (currentBalance + amount > _maxBalance)
                    throw new MaxBalanceExceededException();

                base.Deposit(amount);
            }
        }

        public void ApplyInterest(CompoundingMode mode)
        {
            lock (_interestLock)
            {
                lock (_balanceLock)
                {
                    decimal balance = GetBalance();
                    decimal interest = 0;

                    switch (mode)
                    {
                        case CompoundingMode.Monthly:
                            interest = Math.Round(balance * (_interestRatePercent / 12), 2);
                            break;
                        case CompoundingMode.Yearly:
                            interest = Math.Round(balance * _interestRatePercent, 2);
                            break;
                    }

                    decimal newBalance = balance + interest;

                    if (newBalance > _maxBalance)
                        throw new MaxBalanceExceededException();

                    SetBalance(newBalance);
                    LogTransaction(TransactionType.Interest, interest);
                }
            }
        }
    }
}