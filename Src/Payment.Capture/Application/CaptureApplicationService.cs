using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Capture.Services;
using Payment.EventSourcing.Messages;

namespace Payment.Capture.Application
{
    public class CaptureApplicationService
    {
        private readonly IMediator _mediator;
        private readonly CanVerifyCapture _canVerifyCapture;
        private readonly ILogger<CaptureApplicationService> _logger;

        public CaptureApplicationService(IMediator mediator, CanVerifyCapture canVerifyCapture, ILogger<CaptureApplicationService> logger)
        {
            _mediator = mediator;
            _canVerifyCapture = canVerifyCapture;
            _logger = logger;
        }

        public void Capture(CaptureCommand captureCommand)
        {
            throw new System.NotImplementedException();
        }
    }
}