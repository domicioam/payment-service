using System;
using System.Threading;
using AuthorizeService;
using AuthorizeService.Application;
using AuthorizeService.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthoriseService.UnitTests.Application
{
    public class AuthoriseApplicationServiceTests : IClassFixture<AuthorisationFixture>
    {
        private readonly AuthorisationFixture _fixture;

        public AuthoriseApplicationServiceTests(AuthorisationFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void Should_authorise_request_when_command_is_received()
        {
            var logger = new Mock<ILogger<AuthoriseApplicationService>>();
            var mediator = new Mock<IMediator>();
            var authorisationService = new Mock<AuthorisationService>();
            var cardService = new Mock<CreditCardService>();
            var merchantService = new Mock<MerchantService>();
            
            cardService.Setup(c => c.IsCreditCardValid(It.IsAny<CreditCard>())).Returns(true);
            merchantService.Setup(m => m.IsMerchantValid(It.IsAny<Guid>())).Returns(true);
            
            var authoriseAppService = new AuthoriseApplicationService(logger.Object, mediator.Object, authorisationService.Object, cardService.Object, merchantService.Object);
            authoriseAppService.Authorise(_fixture.Command);
            
            authorisationService.Verify(a => a.CreateAuthorisation(It.IsAny<Guid>(), It.IsAny<CreditCard>(), It.IsAny<Currency>(), It.IsAny<decimal>()), Times.Once);
            mediator.Verify(m => m.Send(It.IsAny<AuthorisationCreated>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public void Should_decline_request_when_merchant_is_invalid()
        {
            var logger = new Mock<ILogger<AuthoriseApplicationService>>();
            var mediator = new Mock<IMediator>();
            var authorisationService = new Mock<AuthorisationService>();
            var cardService = new Mock<CreditCardService>();
            var merchantService = new Mock<MerchantService>();
            
            cardService.Setup(c => c.IsCreditCardValid(It.IsAny<CreditCard>())).Returns(true);
            merchantService.Setup(m => m.IsMerchantValid(It.IsAny<Guid>())).Returns(false);
            
            var authoriseAppService = new AuthoriseApplicationService(logger.Object, mediator.Object, authorisationService.Object, cardService.Object, merchantService.Object);
            authoriseAppService.Authorise(_fixture.Command);
            
            authorisationService.Verify(a => a.CreateAuthorisation(It.IsAny<Guid>(), It.IsAny<CreditCard>(), It.IsAny<Currency>(), It.IsAny<decimal>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<AuthorisationRejected>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public void Should_decline_request_when_card_is_invalid()
        {
            var logger = new Mock<ILogger<AuthoriseApplicationService>>();
            var mediator = new Mock<IMediator>();
            var authorisationService = new Mock<AuthorisationService>();
            var cardService = new Mock<CreditCardService>();
            var merchantService = new Mock<MerchantService>();
            
            cardService.Setup(c => c.IsCreditCardValid(It.IsAny<CreditCard>())).Returns(false);
            merchantService.Setup(m => m.IsMerchantValid(It.IsAny<Guid>())).Returns(true);
            
            var authoriseAppService = new AuthoriseApplicationService(logger.Object, mediator.Object, authorisationService.Object, cardService.Object, merchantService.Object);
            authoriseAppService.Authorise(_fixture.Command);
            
            authorisationService.Verify(a => a.CreateAuthorisation(It.IsAny<Guid>(), It.IsAny<CreditCard>(), It.IsAny<Currency>(), It.IsAny<decimal>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<AuthorisationRejected>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public void Should_decline_request_when_both_card_and_merchant_are_invalid()
        {
            var logger = new Mock<ILogger<AuthoriseApplicationService>>();
            var mediator = new Mock<IMediator>();
            var authorisationService = new Mock<AuthorisationService>();
            var cardService = new Mock<CreditCardService>();
            var merchantService = new Mock<MerchantService>();
            
            cardService.Setup(c => c.IsCreditCardValid(It.IsAny<CreditCard>())).Returns(false);
            merchantService.Setup(m => m.IsMerchantValid(It.IsAny<Guid>())).Returns(false);
            
            var authoriseAppService = new AuthoriseApplicationService(logger.Object, mediator.Object, authorisationService.Object, cardService.Object, merchantService.Object);
            authoriseAppService.Authorise(_fixture.Command);
            
            authorisationService.Verify(a => a.CreateAuthorisation(It.IsAny<Guid>(), It.IsAny<CreditCard>(), It.IsAny<Currency>(), It.IsAny<decimal>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<AuthorisationRejected>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}