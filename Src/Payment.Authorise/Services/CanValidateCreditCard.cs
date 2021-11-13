using System;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace AuthorizeService.Services
{
    public interface CanValidateCreditCard
    {
        bool IsCreditCardValid(CreditCard creditCard, DateTime currentDate);
    }
}