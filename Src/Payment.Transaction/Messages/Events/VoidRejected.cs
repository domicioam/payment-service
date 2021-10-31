using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class VoidRejected: Event, INotification
    {
        public VoidRejected(Guid aggregateId, int version): base(version)
        {
            AggregateId = aggregateId;
        }
    }
}