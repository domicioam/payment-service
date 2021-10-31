using System;
using Payment.EventSourcing.Messages;

namespace Payment.Gateway.Model
{
    public record AuthoriseInput(Guid MerchantId, CreditCard CreditCard, Currency Currency, decimal Amount);
}