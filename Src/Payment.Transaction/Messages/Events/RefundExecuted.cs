using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class RefundExecuted : Event, INotification
    {
        public RefundExecuted(Guid aggregateId, decimal amount, int version): base(version)
        {
            Amount = amount;
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