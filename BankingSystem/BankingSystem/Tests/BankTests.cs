using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankingSystem.BO;
using BankingSystem.Exceptions;
using System;
using System.Linq;

namespace BankingSystem.Tests
{
    [TestClass]
    public class BankTests
    {
        private Bank _bank;

        [TestInitialize]
        public void Setup()
        {
            _bank = new Bank();
        }

        [TestMethod]
        public void OpenAccount_ShouldCreateSavingsAccount()
        {
            var account = _bank.OpenAccount("John Doe", "savings", 1000, 0.05m, 500m);

            Assert.IsNotNull(account);
            Assert.AreEqual(1000, account.GetBalance());
            Assert.AreEqual(1, account.AccountNumber);
        }

        [TestMethod]
        public void OpenAccount_ShouldCreateCustomerIfNotExists()
        {
            _bank.OpenAccount("John Doe", "checking", 500, 200);

            var customers = _bank.GetAllCustomers();

            Assert.AreEqual(1, customers.Count());
            Assert.AreEqual("John Doe", customers.First().Name);
        }

        [TestMethod]
        public void OpenAccount_ShouldThrow_WhenInitialBalanceNegative()
        {
            Assert.ThrowsException<InvalidAmountException>(() =>
            {
                _bank.OpenAccount("John Doe", "savings", -100, 0.05m, 500m);
            });
        }

        [TestMethod]
        public void FindAccount_ShouldReturnCorrectAccount()
        {
            var acc = _bank.OpenAccount("John Doe", "checking", 500, 200);

            var found = _bank.FindAccount(acc.AccountNumber);

            Assert.AreEqual(acc.AccountNumber, found.AccountNumber);
        }

        [TestMethod]
        public void FindAccount_ShouldThrow_WhenNotFound()
        {
            Assert.ThrowsException<AccountNotFoundException>(() =>
            {
                _bank.FindAccount(999);
            });
        }

        [TestMethod]
        public void CloseAccount_ShouldRemoveAccount_WhenBalanceZero()
        {
            var acc = _bank.OpenAccount("John Doe", "checking", 0, 200);

            _bank.CloseAccount(acc.AccountNumber);

            var accounts = _bank.ListAccounts();
            Assert.AreEqual(0, accounts.Count);
        }

        [TestMethod]
        public void CloseAccount_ShouldThrow_WhenBalanceNotZero()
        {
            var acc = _bank.OpenAccount("John Doe", "checking", 100, 200);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _bank.CloseAccount(acc.AccountNumber);
            });
        }

        [TestMethod]
        public void ListAccounts_ShouldReturnAllAccounts()
        {
            _bank.OpenAccount("John Doe", "checking", 100, 200);
            _bank.OpenAccount("Jane Doe", "savings", 200, 0.05m, 100);

            var accounts = _bank.ListAccounts();

            Assert.AreEqual(2, accounts.Count);
        }

        [TestMethod]
        public void GenerateStatement_ShouldThrow_WhenInvalidDateRange()
        {
            var acc = _bank.OpenAccount("John Doe", "checking", 100, 200);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _bank.GenerateStatement(acc.AccountNumber, DateTime.Now, DateTime.Now.AddDays(-1));
            });
        }

        [TestMethod]
        public void FindCustomer_ShouldReturnCorrectCustomer()
        {
            _bank.OpenAccount("John Doe", "checking", 100, 200);
            var customer = _bank.GetAllCustomers().First();

            var found = _bank.FindCustomer(customer.Id);

            Assert.AreEqual(customer.Id, found.Id);
        }

        [TestMethod]
        public void FindCustomer_ShouldThrow_WhenNotFound()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _bank.FindCustomer(999);
            });
        }
    }
}