using System;
using AuthorizeService;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace AuthoriseService.UnitTests
{
    public class AuthorisationFixture
    {
        public DateTime ExpiryDate { get; }
        public CreditCard CreditCard { get; }
        public AuthorisationCommand Command { get; }
        public AuthorisationCommand InvalidCommand { get; }
        

        public AuthorisationFixture()
        {
            ExpiryDate = new DateTime(2023, 10, 20);
            CreditCard = new CreditCard("1234", ExpiryDate, "123");
            Command = new AuthorisationCommand(Guid.NewGuid(), Guid.NewGuid(), CreditCard, Currency.Eur, 10m);
            InvalidCommand= new AuthorisationCommand(Guid.NewGuid(), Guid.NewGuid(), CreditCard, Currency.Eur, 0m);
        }
    }
}