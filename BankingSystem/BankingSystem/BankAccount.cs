namespace BankingSystem
{
    public class BankAccount
    {
        protected string _owner;
        private int _balance = 0;

        public BankAccount(string owner, int balance)
        {
            _owner = owner;
            _balance = balance;
        }

        public int GetBalance()
        {
            return _balance;
        }

        public void SetBalance(int balance)
        {
            if (balance < 0)
                throw new Exception(" invalid input");

            _balance = balance;
        }

        public void Deposit(int amount)
        {
            if(amount <= 0)
                throw new Exception(" invalid input");

            _balance += amount;
        }

        public virtual void Withdraw(int amount)
        {
            if (amount > this._balance)
                throw new Exception(" insufficient funds");

            _balance -= amount;
        }

        public void TransferTo(BankAccount account, int amount)
        { 
            Withdraw(amount);
            account.Deposit(amount);
        }
    }
}
