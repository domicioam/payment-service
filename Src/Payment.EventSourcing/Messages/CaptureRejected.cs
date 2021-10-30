using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class CaptureRejected : VersionedEvent, INotification
    {
        public CaptureRejected(Guid aggregateId, int version): base(version)
        {
            AggregateId = aggregateId;
        }
    }
}