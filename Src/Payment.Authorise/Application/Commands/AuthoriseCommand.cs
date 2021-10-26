using System;

namespace AuthorizeService.Application
{
    public record AuthorisationCommand(Guid MerchantId, CreditCard CreditCard, Currency Currency, decimal Amount);
}