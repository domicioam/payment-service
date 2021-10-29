using System;
using AuthorizeService;
using AuthorizeService.Application;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace AuthoriseService.UnitTests
{
    public class AuthorisationFixture
    {
        public DateTime ExpiryDate { get; }
        public CreditCard CreditCard { get; }
        public AuthorisationCommand Command { get; }

        public AuthorisationFixture()
        {
            ExpiryDate = new DateTime(2023, 10, 20);
            CreditCard = new CreditCard("1234", ExpiryDate, "123");
            Command = new AuthorisationCommand(Guid.NewGuid(), CreditCard, Currency.Eur, 10m);
        }
    }
}