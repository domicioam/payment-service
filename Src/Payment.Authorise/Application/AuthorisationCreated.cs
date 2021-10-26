using System;

namespace AuthorizeService.Application
{
    public record AuthorisationCreated(Guid MerchantId, Guid AuthorisationId);
}