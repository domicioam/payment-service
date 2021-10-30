using System;
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
            var (authorisationId, amount) = captureCommand;
            _logger.LogInformation($"[Capture] Capture started for authorisation: {authorisationId}");
                
            try
            {
                var canCapture = _canVerifyCapture.CanExecuteCapture(authorisationId, amount);
                if (canCapture)
                {
                    _mediator.Publish(new CaptureExecuted(authorisationId, amount));
                    _logger.LogInformation($"[Capture] Capture accepted for authorisation: {authorisationId}");
                    return;
                }

                _mediator.Publish(new CaptureRejected(authorisationId));
                _logger.LogWarning($"[Capture] Capture rejected for authorisation id: {authorisationId}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[Capture] Error when trying to capture for authorisation id: {authorisationId}.");
            }
        }
    }
}