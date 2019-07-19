using System;

namespace Moneybox.App
{
    public class Account
    {
        private const decimal PayInLimit = 4000m;

        private const decimal FundsLowNotificationLimit = 500m;

        private const decimal PayInLimitNotificationLimit = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool AreFundsLow => Balance < FundsLowNotificationLimit;

        public bool IsApproachingPayInLimit => (PayInLimit - PaidIn) < PayInLimitNotificationLimit;

        public void WithdrawMoney(decimal amount)
        {
            var balanceAmount = Balance - amount;

            if (balanceAmount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            Balance = balanceAmount;
            Withdrawn = Withdrawn - amount;
        }

        public void PayInMoney(decimal amount)
        {
            var paidInAmount = PaidIn + amount;

            if (paidInAmount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            Balance = Balance + amount;
            PaidIn = paidInAmount;
        }
    }
}
