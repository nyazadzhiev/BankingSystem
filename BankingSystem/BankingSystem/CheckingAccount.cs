namespace BankingSystem
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
                throw new Exception("Overdraft exceeded");

            ResetDailyLimit();

            if (_withdrawnToday + amount > _dailyWithdrawalLimit)
                throw new Exception(" daily limit exceeded");

            SetBalance(newBalance);

            LogTransaction(TransactionType.Withdraw, amount);
        }
    }
}
