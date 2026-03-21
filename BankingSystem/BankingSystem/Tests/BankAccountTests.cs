using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankingSystem.BO;
using BankingSystem.Exceptions;

namespace BankingSystem.Tests
{
    [TestClass]
    public class BankAccountTests
    {
        private CheckingAccount _checking;
        private SavingsAccount _savings;

        [TestInitialize]
        public void Setup()
        {
            _checking = new CheckingAccount("John", 1, 1000, 500);
            _savings = new SavingsAccount("Jane", 2, 1000, 0.12m, 5000);
        }

        [TestMethod]
        public void Deposit_ShouldIncreaseBalance()
        {
            _checking.Deposit(500);

            Assert.AreEqual(1500, _checking.GetBalance());
        }

        [TestMethod]
        public void Deposit_ShouldThrow_WhenNegative()
        {
            Assert.ThrowsException<InvalidAmountException>(() =>
            {
                _checking.Deposit(-10);
            });
        }

        [TestMethod]
        public void Savings_Deposit_ShouldThrow_WhenExceedMaxBalance()
        {
            Assert.ThrowsException<MaxBalanceExceededException>(() =>
            {
                _savings.Deposit(10000);
            });
        }

        [TestMethod]
        public void Withdraw_ShouldDecreaseBalance()
        {
            _checking.Withdraw(200);

            Assert.AreEqual(800, _checking.GetBalance());
        }

        [TestMethod]
        public void Withdraw_ShouldThrow_WhenInsufficientFunds_Savings()
        {
            Assert.ThrowsException<InsufficientFundsException>(() =>
            {
                _savings.Withdraw(2000);
            });
        }

        [TestMethod]
        public void Withdraw_ShouldThrow_WhenInvalidAmount()
        {
            Assert.ThrowsException<InvalidAmountException>(() =>
            {
                _checking.Withdraw(-50);
            });
        }

        [TestMethod]
        public void Checking_ShouldAllowOverdraftWithinLimit()
        {
            _checking.Withdraw(1300); // 1000 - 1300 = -300 (allowed)

            Assert.IsTrue(_checking.GetBalance() < 0);
        }

        [TestMethod]
        public void Checking_ShouldThrow_WhenOverdraftExceeded()
        {
            Assert.ThrowsException<OverdraftLimitExceededException>(() =>
            {
                _checking.Withdraw(2000);
            });
        }


        [TestMethod]
        public void Transfer_ShouldMoveMoneyBetweenAccounts()
        {
            var target = new CheckingAccount("Bob", 3, 500, 200);

            _checking.TransferTo(target, 300);

            Assert.AreEqual(700, _checking.GetBalance());
            Assert.AreEqual(800, target.GetBalance());
        }

        [TestMethod]
        public void Transfer_ShouldThrow_WhenInvalidAmount()
        {
            var target = new CheckingAccount("Bob", 3, 500, 200);

            Assert.ThrowsException<InvalidAmountException>(() =>
            {
                _checking.TransferTo(target, -100);
            });
        }


        [TestMethod]
        public void ApplyInterest_ShouldIncreaseBalance_Monthly()
        {
            _savings.ApplyInterest(CompoundingMode.Monthly);

            Assert.IsTrue(_savings.GetBalance() > 1000);
        }

        [TestMethod]
        public void ApplyInterest_ShouldIncreaseBalance_Yearly()
        {
            _savings.ApplyInterest(CompoundingMode.Yearly);

            Assert.IsTrue(_savings.GetBalance() > 1000);
        }

        [TestMethod]
        public void ApplyInterest_ShouldThrow_WhenMaxBalanceExceeded()
        {
            var rich = new SavingsAccount("Rich", 4, 4900, 0.5m, 5000);

            Assert.ThrowsException<MaxBalanceExceededException>(() =>
            {
                rich.ApplyInterest(CompoundingMode.Yearly);
            });
        }


        [TestMethod]
        public void Deposit_ShouldCreateTransaction()
        {
            _checking.Deposit(100);

            Assert.IsTrue(_checking.Transactions.Any(t => t.Type == TransactionType.Deposit));
        }

        [TestMethod]
        public void Withdraw_ShouldCreateTransaction()
        {
            _checking.Withdraw(100);

            Assert.IsTrue(_checking.Transactions.Any(t => t.Type == TransactionType.Withdraw));
        }
    }
}