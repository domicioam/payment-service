using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class CaptureCompleted : Event, INotification
    {
        public CaptureCompleted(Guid aggregateId, int version) : base(version)
        {
            AggregateId = aggregateId;
        }
    }
}