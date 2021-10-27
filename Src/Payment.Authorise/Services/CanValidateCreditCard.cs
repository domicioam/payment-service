using System;

namespace AuthorizeService.Services
{
    public interface CanValidateCreditCard
    {
        bool IsCreditCardValid(CreditCard creditCard, DateTime validUntil);
    }
}