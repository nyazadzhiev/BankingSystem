namespace BankingSystem.BO
{
    public class Customer
    {
        public int Id { get; }
        public string Name { get; }
        public List<BankAccount> Accounts { get; } = new();

        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
