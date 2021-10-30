using System;
using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Payment.Capture.Application;
using Payment.Capture.Services;
using Payment.EventSourcing.Messages;
using Xunit;

namespace Payment.Capture.UnitTests.Application
{
    public class CaptureApplicationServiceTests
    {
        [Fact]
        public void Should_send_event_to_capture_when_capture_is_allowed()
        {
            var logger = new Mock<ILogger<CaptureApplicationService>>();
            var mediator = new Mock<IMediator>();
            var captureVerificator = new Mock<CanVerifyCapture>();
            captureVerificator.Setup(c => c.CanExecuteCapture(It.IsAny<Guid>(), It.IsAny<decimal>())).Returns(true);
            var captureApplicationService = new CaptureApplicationService(mediator.Object, captureVerificator.Object, logger.Object);
            var authorisationId = Guid.NewGuid();
            decimal amount = 10m;
            var captureCommand = new CaptureCommand(authorisationId, amount);
            captureApplicationService.Capture(captureCommand);
            
            captureVerificator.Verify(c => c.CanExecuteCapture(authorisationId, amount), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<CaptureExecuted>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Should_not_send_capture_event_when_cant_execute_capture()
        {
            var logger = new Mock<ILogger<CaptureApplicationService>>();
            var mediator = new Mock<IMediator>();
            var captureVerificator = new Mock<CanVerifyCapture>();
            captureVerificator.Setup(c => c.CanExecuteCapture(It.IsAny<Guid>(), It.IsAny<decimal>())).Returns(false);
            var captureApplicationService = new CaptureApplicationService(mediator.Object, captureVerificator.Object, logger.Object);
            var authorisationId = Guid.NewGuid();
            decimal amount = 10m;
            var captureCommand = new CaptureCommand(authorisationId, amount);
            captureApplicationService.Capture(captureCommand);
            
            captureVerificator.Verify(c => c.CanExecuteCapture(authorisationId, amount), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<CaptureRejected>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<CaptureExecuted>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public void Should_not_send_capture_event_when_exception_thrown()
        {
            var logger = new Mock<ILogger<CaptureApplicationService>>();
            var mediator = new Mock<IMediator>();
            var captureVerificator = new Mock<CanVerifyCapture>();
            captureVerificator.Setup(c => c.CanExecuteCapture(It.IsAny<Guid>(), It.IsAny<decimal>())).Throws(new Exception());
            var captureApplicationService = new CaptureApplicationService(mediator.Object, captureVerificator.Object, logger.Object);
            var authorisationId = Guid.NewGuid();
            decimal amount = 10m;
            var captureCommand = new CaptureCommand(authorisationId, amount);
            captureApplicationService.Capture(captureCommand);
            
            captureVerificator.Verify(c => c.CanExecuteCapture(authorisationId, amount), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<CaptureRejected>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<CaptureExecuted>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}