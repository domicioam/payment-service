using System;

namespace AuthorizeService
{
    public record CreditCard(string Number, DateTime ExpiryDate, string Cvv)
    {
        public bool IsValid(DateTime validUntil)
        {
            // silly validation
            return !string.IsNullOrWhiteSpace(Number) && !string.IsNullOrWhiteSpace(Cvv)
                                                      && validUntil > ExpiryDate;
        }
    }
}