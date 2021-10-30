using System;

namespace Payment.EventSourcing.Messages
{
    public record CaptureExecuted(Guid AuthorisationId, decimal Amount);
}