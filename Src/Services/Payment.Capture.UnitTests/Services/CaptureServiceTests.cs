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
        public void test()
        {
            var authorisationId = Guid.NewGuid();
            decimal totalAllowed = 25m;
            var captureRepository = new Mock<CaptureRepository>();
            captureRepository.Setup(c => c.GetAmountLeftToCapture(authorisationId)).Returns(totalAllowed);
            var captureService = new CaptureService(captureRepository.Object);
        }
    }
}