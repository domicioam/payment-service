using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class CaptureExecuted : VersionedEvent, INotification
    {
        public CaptureExecuted(Guid aggregateId, decimal Amount, int version): base(version)
        {
            this.Amount = Amount;
            AggregateId = aggregateId;
        }

        public decimal Amount { get; init; }

        public void Deconstruct(out Guid AggregateId, out decimal Amount)
        {
            Amount = this.Amount;
            AggregateId = this.AggregateId;
        }
    }
}