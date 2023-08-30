using NUnit.Framework;
using System;

namespace AwesomeGIC_BankTest
{
    [TestFixture]
    public class BankingSystemTests
    {
        private BankingSystem bankingSystem;

        [SetUp]
        public void SetUp()
        {
            bankingSystem = new BankingSystem();
        }


        [Test]
        public void ProcessTransaction_WithValidInput_ShouldAddTransaction()
        {
            string input = "20230101|Account1|D|100";
            bankingSystem.ProcessTransaction(input);
            Assert.AreEqual(1, bankingSystem.transactions.Count);
        }

        [Test]
        public void ProcessTransaction_WithInvalidInput_ShouldNotAddTransaction()
        {
            string input = "20230101|Account1|X|100";
            bankingSystem.ProcessTransaction(input);
            Assert.AreEqual(0, bankingSystem.transactions.Count);
        }

        [Test]
        public void ProcessInterestRule_WithValidInput_ShouldAddInterestRule()
        {
            string input = "20230101|Rule1|5";
            bankingSystem.ProcessInterestRule(input);
            Assert.AreEqual(1, bankingSystem.interestRules.Count);
        }

        [Test]
        public void ProcessInterestRule_WithInvalidInput_ShouldNotAddInterestRule()
        {
            string input = "20230101|Rule1|105"; 
            bankingSystem.ProcessInterestRule(input);
            Assert.AreEqual(0, bankingSystem.interestRules.Count);
        }

        [Test]
        public void CalculateAndDisplayInterest_WithNoTransactions_ShouldNotCalculateInterest()
        {
            bankingSystem.CalculateAndDisplayInterest("Account1");
        }


        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}