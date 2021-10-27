using System;
using AuthorizeService.Application;
using AuthorizeService.Entities;

namespace AuthorizeService.Factories
{
    public class AuthorisationFactory
    {
        public virtual Authorisation CreateAuthorisation(Guid merchantId, CreditCard creditCard, Currency currency, decimal amount)
        {
            return new Authorisation(Guid.NewGuid());
        }
    }
}