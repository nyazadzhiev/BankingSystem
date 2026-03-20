namespace BankingSystem
{
    public class SavingsAccount : BankAccount
    {
        private int _interestRate;

        public SavingsAccount(string owner, int balance, int interestRate) : base(owner, balance)
        {
            _interestRate = interestRate;
        }

        public void applyInterest()
        {
            int balance = getBalance();
            setBalance(balance + (balance * _interestRate));
        }
    }
}
