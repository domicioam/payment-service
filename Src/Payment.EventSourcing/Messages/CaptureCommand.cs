using System;

namespace Payment.EventSourcing.Messages
{
    public record CaptureCommand(Guid AggregateId, decimal Amount);
}