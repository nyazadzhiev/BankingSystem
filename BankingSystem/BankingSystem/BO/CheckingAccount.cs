using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class CheckingAccount : BankAccount
    {
        private int _overdraftLimit;
        public CheckingAccount(string owner, int balance, int overdraftLimit) : base(owner, balance)
        {
            _overdraftLimit = overdraftLimit;
        }

        public override void Withdraw(int amount)
        {
            int newBalance = GetBalance() - amount;
            if (newBalance < -_overdraftLimit)
                throw new OverdraftLimitExceededException();

            ResetDailyLimit();

            if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                throw new DailyLimitExceededException();

            SetBalance(newBalance);

            LogTransaction(TransactionType.Withdraw, amount);
        }
    }
}
