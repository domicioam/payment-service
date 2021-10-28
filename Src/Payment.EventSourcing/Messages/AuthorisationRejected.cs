using System;
using MediatR;
using Payment.Foundation.EventSourcing;

namespace Payment.EventSourcing.Messages
{
    public class AuthorisationRejected : Event, INotification
    {
        public AuthorisationRejected(Guid MerchantId, string CreditCardNumber)
        {
            this.MerchantId = MerchantId;
            this.CreditCardNumber = CreditCardNumber;
        }

        public Guid MerchantId { get; init; }
        public string CreditCardNumber { get; init; }

        public void Deconstruct(out Guid MerchantId, out string CreditCardNumber)
        {
            MerchantId = this.MerchantId;
            CreditCardNumber = this.CreditCardNumber;
        }
    }
}