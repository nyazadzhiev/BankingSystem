namespace BankingSystem.Exceptions
{
    public class OverdraftLimitExceededException : Exception
    {
        public OverdraftLimitExceededException() : base("Overdraft limit exceeded.") { }
    }
}
