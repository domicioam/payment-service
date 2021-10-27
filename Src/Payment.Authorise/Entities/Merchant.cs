using System;

namespace AuthorizeService.Entities
{
    public record Merchant(Guid Id, bool IsActive);
}