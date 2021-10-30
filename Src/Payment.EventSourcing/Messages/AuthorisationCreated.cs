using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class AuthorisationCreated : Event, INotification
    {
        public AuthorisationCreated(Guid MerchantId, Guid AuthorisationId, decimal amount)
        {
            this.MerchantId = MerchantId;
            AggregateId = AuthorisationId;
            Amount = amount;
        }

        public decimal Amount { get; init; }

        public Guid MerchantId { get; init; }

        public void Deconstruct(out Guid MerchantId, out Guid AuthorisationId, out decimal Amount)
        {
            MerchantId = this.MerchantId;
            AuthorisationId = this.AggregateId;
            Amount = this.Amount;
        }
    }
}