using System;

namespace Payment.EventSourcing.Messages
{
    public record CaptureCommand(Guid AuthorisationId, decimal Amount);
}