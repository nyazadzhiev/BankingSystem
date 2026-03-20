using BankingSystem.Exceptions;

namespace BankingSystem.BO
{
    public class SavingsAccount : BankAccount
    {
        private int _interestRate;
        private int _maxBalance;

        public SavingsAccount(string owner, int balance, int interestRate, int maxBalance) : base(owner, balance)
        {
            _interestRate = interestRate;
            _maxBalance = maxBalance;
        }

        public void ApplyInterest()
        {
            int balance = GetBalance();
            int newBalance = balance + balance * _interestRate;

            if (newBalance > _maxBalance)
                throw new OverdraftLimitExceededException();

            SetBalance(newBalance);

            LogTransaction(TransactionType.Interest, _interestRate);
        }
    }
}
