using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accountRepository.GetAccountById(fromAccountId);
            var toAccount = _accountRepository.GetAccountById(toAccountId);

            fromAccount.WithdrawMoney(amount);
            toAccount.PayInMoney(amount);

            if (fromAccount.AreFundsLow)
            {
                _notificationService.NotifyFundsLow(fromAccount.User.Email);
            }

            if (toAccount.IsApproachingPayInLimit)
            {
                _notificationService.NotifyApproachingPayInLimit(toAccount.User.Email);
            }

            _accountRepository.Update(fromAccount);
            _accountRepository.Update(toAccount);
        }
    }
}
