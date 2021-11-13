using System;

namespace Payment.EventSourcing.Messages
{
    public record CreditCard(string Number, DateTime ExpiryDate, string Cvv)
    {
        public bool IsValid(DateTime currentDate)
        {
            // silly validation
            return !string.IsNullOrWhiteSpace(Number) && !string.IsNullOrWhiteSpace(Cvv)
                                                      && ExpiryDate > currentDate;
        }
    }
}