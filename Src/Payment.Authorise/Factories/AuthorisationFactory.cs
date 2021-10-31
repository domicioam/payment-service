using System;
using AuthorizeService.Entities;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace AuthorizeService.Factories
{
    public class AuthorisationFactory
    {
        public virtual Authorisation CreateAuthorisation(Guid transactionId, Guid merchantId, CreditCard creditCard, Currency currency, decimal amount)
        {
            return new Authorisation(transactionId);
        }
    }
}