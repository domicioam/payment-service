using System;

namespace AuthorizeService.Application
{
    public record AuthorisationRejected(Guid MerchantId, string CreditCardNumber);
}