using System;

namespace Payment.Foundation.EventSourcing
{
    public class Message
    {
        public Guid AggregateId { get; protected set; }
        public string Name { get; protected set; }
    }
}
