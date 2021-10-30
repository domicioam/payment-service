using System;

namespace Payment.EventSourcing.Messages
{
    public record AuthorisationCommand(Guid MerchantId, CreditCard CreditCard, Currency Currency, decimal Amount);
}