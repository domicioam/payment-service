using System;
using System.Threading.Tasks;
using AuthorizeService;
using AuthorizeService.Entities;
using AuthorizeService.Repository;
using AuthorizeService.Services;
using Moq;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;
using Xunit;

namespace AuthoriseService.UnitTests.Services
{
    public class AuthorisationServiceTests
    {
        [Fact]
        public async Task Should_return_true_when_merchant_is_active()
        {
            var merchantRepository = new Mock<MerchantRepository>();
            var id = Guid.NewGuid();
            merchantRepository.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Merchant(id, true));
            var authorisationService = new AuthorisationService(merchantRepository.Object);
            var result = await authorisationService.IsMerchantValidAsync(id);
            Assert.True(result);
        }
        
        [Fact]
        public async Task Should_return_false_when_merchant_is_inactive()
        {
            var merchantRepository = new Mock<MerchantRepository>();
            var id = Guid.NewGuid();
            merchantRepository.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Merchant(id, false));
            var authorisationService = new AuthorisationService(merchantRepository.Object);
            var result = await authorisationService.IsMerchantValidAsync(id);
            Assert.False(result);
        }
        
        [Fact]
        public void Should_return_true_when_card_is_valid()
        {
            var merchantRepository = new Mock<MerchantRepository>();
            var creditCard = new CreditCard("1234", new DateTime(2023, 10, 20), "123");
            var validUntil = new DateTime(2024, 10, 20);
            var authorisationService = new AuthorisationService(merchantRepository.Object);
            var result = authorisationService.IsCreditCardValid(creditCard, validUntil);
            Assert.True(result);
        }
        
        [Fact]
        public void Should_return_false_when_card_is_invalid()
        {
            var merchantRepository = new Mock<MerchantRepository>();
            var creditCard = new CreditCard("1234", new DateTime(2025, 10, 20), "123");
            var validUntil = new DateTime(2024, 10, 20);
            var authorisationService = new AuthorisationService(merchantRepository.Object);
            var result = authorisationService.IsCreditCardValid(creditCard, validUntil);
            Assert.False(result);
        }
    }
}