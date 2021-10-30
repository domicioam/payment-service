using MediatR;
using Payment.Capture.Services;
using Payment.EventSourcing.Messages;

namespace Payment.Capture.Application
{
    public class CaptureApplicationService
    {
        public CaptureApplicationService(IMediator mediator, CanVerifyCapture canVerifyCapture)
        {
            throw new System.NotImplementedException();
        }

        public void Capture(CaptureCommand captureCommand)
        {
            throw new System.NotImplementedException();
        }
    }
}