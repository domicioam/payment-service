using System;

namespace Payment.EventSourcing.Messages
{
    public record RefundCommand(Guid AggregateId, decimal Amount);
}