using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.Features
{
    [TestFixture]
    public class WithdrawMoneyTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository = new Mock<IAccountRepository>();
        private readonly Mock<INotificationService> _mockNotificationService = new Mock<INotificationService>();

        private WithdrawMoney _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new WithdrawMoney(_mockAccountRepository.Object, _mockNotificationService.Object);
        }

        [Test]
        public void Execute_SuccessfulWithdraw_UpdatesAccount()
        {
            //Arrange
            var amount = 100m;

            var fromAccount = new Account
            {
                Balance = 200m,
                Withdrawn = 250m,
                PaidIn = 0m,
                User = new User
                {
                    Email = "joe.bloggs@email.com"
                }
            };

            _mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(fromAccount);

            //Act
            _sut.Execute(Guid.NewGuid(), amount);

            //Assert
            Assert.AreEqual(100, fromAccount.Balance);
        }

        [Test]
        public void Execute_FundsLow_NotifiesFundsLow()
        {
            //Arrange
            var amount = 100m;

            var fromAccount = new Account
            {
                Balance = 200m,
                Withdrawn = 250m,
                PaidIn = 0m,
                User = new User
                {
                    Email = "joe.bloggs@email.com"
                }
            };

            _mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(fromAccount);

            //Act
            _sut.Execute(Guid.NewGuid(), amount);

            //Assert
            _mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.AtLeastOnce);
        }

    }
}
