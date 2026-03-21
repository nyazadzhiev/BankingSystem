namespace BankingSystem.Exceptions
{
    public class MaxBalanceExceededException : Exception
    {
        public MaxBalanceExceededException() : base("Maximum balance limit exceeded.") { }
    }
}
