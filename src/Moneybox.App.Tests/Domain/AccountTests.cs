using NUnit.Framework;
using System;

namespace Moneybox.App.Tests
{
    [TestFixture]
    public class AccountTests
    {
        private Account _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Account();
        }

        [TestCase(-100, true)]
        [TestCase(10, true)]
        [TestCase(550, false)]
        public void IsFundsLow_TestCases_ShouldMatchExpected(double balance, bool expected)
        {
            //Arrange
            var balanceDecimal = Convert.ToDecimal(balance);
            _sut.Balance = balanceDecimal;

            //Act
            var actual = _sut.AreFundsLow;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(500, false)]
        [TestCase(3500, false)]
        [TestCase(3850, true)]
        [TestCase(4250, true)]
        public void IsApproachingPayInLimit_TestCases_ShouldMatchExpected(double paidIn, bool expected)
        {
            //Arrange
            var paidInDecimal = Convert.ToDecimal(paidIn);
            _sut.PaidIn = paidInDecimal;

            //Act
            var actual = _sut.IsApproachingPayInLimit;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PayInMoney_PaidInExceedsLimit_ThrowsInvalidOperationExceptionAndMessageMatches()
        {
            //Arrange
            _sut.PaidIn = 500m;
            var amount = 3750m;

            //Act/Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _sut.PayInMoney(amount));
            Assert.AreEqual("Account pay in limit reached", ex.Message);
        }

        [Test]
        public void PayInMoney_SetsBalanceAndPaidInFields()
        {
            //Arrange
            _sut.Balance = 500m;
            _sut.PaidIn = 250m;

            var amount = 100m;

            //Act
            _sut.PayInMoney(amount);

            //Assert
            Assert.AreEqual(350m, _sut.PaidIn);
            Assert.AreEqual(600m, _sut.Balance);
        }

        [Test]
        public void WithdrawMoney_InsufficientFunds_ThrowsInvalidOperationExceptionAndMessageMatches()
        {
            //Arrange
            _sut.Balance = 50;
            var amount = 100m;

            //Act/Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _sut.WithdrawMoney(amount));
            Assert.AreEqual("Insufficient funds to make transfer", ex.Message);
        }

        [Test]
        public void WithdrawMoney_SetsBalanceAndWithdrawnFields()
        {
            //Arrange
            _sut.Balance = 500m;
            _sut.Withdrawn = 400m;

            var amount = 100m;

            //Act
            _sut.WithdrawMoney(amount);

            //Assert
            Assert.AreEqual(300m, _sut.Withdrawn);
            Assert.AreEqual(400m, _sut.Balance);
        }

    }
}
