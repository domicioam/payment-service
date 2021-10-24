using System;

namespace AuthorizeService
{
    public record Authorisation(Guid Id, bool Success, decimal Amount, Currency Currency);

    public enum Currency
    {
        Eur,
        Usd
    }
}