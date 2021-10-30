using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class CaptureRejected : Event, INotification
    {
        public CaptureRejected(Guid aggregateId, int version): base(version)
        {
            AggregateId = aggregateId;
        }
    }
}