using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class RefundCompleted: Event, INotification
    {
        public RefundCompleted(Guid aggregateId, int version) : base(version)
        {
            AggregateId = aggregateId;
        }
    }
}