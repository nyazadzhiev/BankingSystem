using System;

namespace BankingSystem.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException() : base("Insufficient funds.") { }
    }

}
