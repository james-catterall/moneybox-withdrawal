using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.Features
{
    [TestFixture]
    public class TransferMoneyTests
    {
        private Guid _fromAccountId;
        private Guid _toAccountId;

        private readonly Mock<IAccountRepository> _mockAccountRepository = new Mock<IAccountRepository>();
        private readonly Mock<INotificationService> _mockNotificationService = new Mock<INotificationService>();

        private TransferMoney _sut;

        [SetUp]
        public void SetUp()
        {
            _fromAccountId = Guid.NewGuid();
            _toAccountId = Guid.NewGuid();

            _sut = new TransferMoney(_mockAccountRepository.Object, _mockNotificationService.Object);
        }

        [Test]
        public void Execute_SuccessfulTransfer_UpdatesAccounts()
        {
            //Arrange
            var amount = 150m;

            var fromAccount = new Account
            {
                Balance = 900m,
                Withdrawn = 250m,
                PaidIn = 0m,
                User = new User
                {
                    Email = "joe.bloggs@email.com"
                }
            };

            var toAccount = new Account
            {
                Balance = 750m,
                Withdrawn = 250m,
                PaidIn = 0m,
                User = new User
                {
                    Email = "john.doe@email.com"
                }
            };

            _mockAccountRepository.Setup(x => x.GetAccountById(_fromAccountId)).Returns(fromAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(_toAccountId)).Returns(toAccount);

            //Act
            _sut.Execute(_fromAccountId, _toAccountId, amount);

            //Assert
            Assert.AreEqual(750m, fromAccount.Balance);
            Assert.AreEqual(900m, toAccount.Balance);
        }

        [Test]
        public void Execute_FundsLow_NotifiesFundsLow()
        {
            //Arrange
            var amount = 150m;

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

            var toAccount = new Account
            {
                Balance = 500m,
                Withdrawn = 250m,
                PaidIn = 0m,
                User = new User
                {
                    Email = "john.doe@email.com"
                }
            };

            _mockAccountRepository.Setup(x => x.GetAccountById(_fromAccountId)).Returns(fromAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(_toAccountId)).Returns(toAccount);

            //Act
            _sut.Execute(_fromAccountId, _toAccountId, amount);

            //Assert
            _mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ApproachingPayInLimit_NotifiesApproachingPayInLimit()
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

            var toAccount = new Account
            {
                Balance = 500m,
                Withdrawn = 250m,
                PaidIn = 3600m,
                User = new User
                {
                    Email = "john.doe@email.com"
                }
            };

            _mockAccountRepository.Setup(x => x.GetAccountById(_fromAccountId)).Returns(fromAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(_toAccountId)).Returns(toAccount);

            //Act
            _sut.Execute(_fromAccountId, _toAccountId, amount);

            //Assert
            _mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
        }

    }
}
