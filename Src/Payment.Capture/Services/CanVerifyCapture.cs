using System;

namespace Payment.Capture.Services
{
    public interface CanVerifyCapture
    {
        bool CanExecuteCapture(Guid authorisationId, decimal amount);
    }
}