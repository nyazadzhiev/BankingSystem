using System;

namespace BankingSystem.Exceptions
{
    public class DailyLimitExceededException : Exception
    {
        public DailyLimitExceededException() : base("Daily withdrawal limit exceeded.") { }
    }
}
