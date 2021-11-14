using System;
using System.Text.Json.Serialization;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class AuthorisationCreated : Event, INotification
    {
        public AuthorisationCreated(Guid MerchantId, Guid AuthorisationId, decimal amount): base(version: 0)
        {
            this.MerchantId = MerchantId;
            AggregateId = AuthorisationId;
            Amount = amount;
        }

        [JsonConstructor]
        public AuthorisationCreated(decimal amount, Guid merchantId, DateTime when, int version, Guid aggregateId, string name): base(version)
        {
            Amount = amount;
            MerchantId = merchantId;
            When = when;
            Version = version;
            AggregateId = aggregateId;
            Name = name;
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