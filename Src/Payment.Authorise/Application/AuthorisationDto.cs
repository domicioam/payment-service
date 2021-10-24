using System;

namespace AuthorizeService.Application
{
    public record AuthorisationDto(Guid Id, bool Success, decimal Amount, Currency Currency);

    public enum Currency
    {
        Eur,
        Usd
    }
}