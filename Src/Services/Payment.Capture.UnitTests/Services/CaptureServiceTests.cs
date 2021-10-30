using System;
using Moq;
using Payment.Capture.Repository;
using Payment.Capture.Services;
using Xunit;

namespace Payment.Capture.UnitTests.Services
{
    public class CaptureServiceTests
    {
        [Fact]
        public void Should_allow_capture_when_amount_is_less_than_total_amount()
        {
            var authorisationId = Guid.NewGuid();
            decimal totalAllowed = 25m;
            decimal amount = 10m;
            var captureRepository = new Mock<CaptureRepository>();
            captureRepository.Setup(c => c.GetAmountLeftToCapture(authorisationId)).Returns(totalAllowed);
            var captureService = new CaptureService(captureRepository.Object);
            bool canCapture = captureService.CanExecuteCapture(authorisationId, amount);
            Assert.True(canCapture);
        }
        
        [Fact]
        public void Should_not_allow_capture_when_amount_is_greater_than_total_amount()
        {
            var authorisationId = Guid.NewGuid();
            decimal totalAllowed = 25m;
            decimal amount = 30m;
            var captureRepository = new Mock<CaptureRepository>();
            captureRepository.Setup(c => c.GetAmountLeftToCapture(authorisationId)).Returns(totalAllowed);
            var captureService = new CaptureService(captureRepository.Object);
            bool canCapture = captureService.CanExecuteCapture(authorisationId, amount);
            Assert.False(canCapture);
        }
    }
}