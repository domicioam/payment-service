using System;
using AuthorizeService.Application;

namespace AuthorizeService.Services
{
    public interface AuthorisationService
    {
        Authorisation CreateAuthorisation(Guid merchantId, CreditCard creditCard, Currency currency, decimal amount);
    }
}