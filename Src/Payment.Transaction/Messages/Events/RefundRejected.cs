using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class RefundRejected : Event, INotification
    {
        public RefundRejected(Guid aggregateId, int version): base(version)
        {
            AggregateId = aggregateId;
        }
    }
}