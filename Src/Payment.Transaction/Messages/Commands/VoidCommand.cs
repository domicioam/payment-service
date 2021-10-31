using System;

namespace Payment.EventSourcing.Messages
{
    public record VoidCommand(Guid AggregateId);
}