namespace BankingSystem
{
    public class SavingsAccount : BankAccount
    {
        private int _interestRate;

        public SavingsAccount(string owner, int balance, int interestRate) : base(owner, balance)
        {
            _interestRate = interestRate;
        }

        public void ApplyInterest()
        {
            int balance = GetBalance();
            SetBalance(balance + (balance * _interestRate));

            LogTransaction(TransactionType.Interest, _interestRate);
        }
    }
}
