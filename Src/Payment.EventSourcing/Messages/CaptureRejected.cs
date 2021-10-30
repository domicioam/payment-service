using System;

namespace Payment.EventSourcing.Messages
{
    public record CaptureRejected(Guid authorisationId);
}