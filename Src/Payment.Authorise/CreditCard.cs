using System;

namespace AuthorizeService
{
    public record CreditCard(string Number, DateTime ExpiryDate, string Cvv);
}