using System;
using MediatR;

namespace Payment.EventSourcing.Messages
{
    public class TransactionVoided: Event, INotification
    {
        public TransactionVoided(Guid aggregateId, int version): base(version)
        {
            AggregateId = aggregateId;
        }
    }
}