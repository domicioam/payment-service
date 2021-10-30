using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class CaptureExecuted : Event, INotification
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
        
        protected bool Equals(CaptureExecuted other)
        {
            return Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CaptureExecuted) obj);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }
    }
}