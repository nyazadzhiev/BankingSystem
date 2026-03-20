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

        public void ApplyInterest(CompoundingMode mode)
        {
            double balance = GetBalance();
            double interest = 0;

            switch (mode)
            {
                case CompoundingMode.Monthly:
                    interest = balance * (_interestRate / 12);
                    break;

                case CompoundingMode.Yearly:
                    interest = balance * _interestRate;
                    break;
            }

            double newBalance = balance + interest;

            if (newBalance > _maxBalance)
                throw new InsufficientFundsException();

            SetBalance((int)newBalance);

            LogTransaction(TransactionType.Interest, (int)interest);
        }
    }
}
