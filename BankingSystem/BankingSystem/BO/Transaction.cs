namespace BankingSystem.BO
{
    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Transfer,
        Interest,
        Fee
    }

    public class Transaction
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public int? FromAccount { get; set; }
        public int? ToAccount { get; set; }
        public decimal ResultingBalance { get; set; }
    }
}
