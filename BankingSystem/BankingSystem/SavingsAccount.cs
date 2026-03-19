namespace BankingSystem
{
    public class SavingsAccount : BankAccount
    {
        private int interestRate;

        public void applyInterest()
        {
            int balance = getBalance();
            setBalance(balance + (balance * interestRate));
        }
    }
}
