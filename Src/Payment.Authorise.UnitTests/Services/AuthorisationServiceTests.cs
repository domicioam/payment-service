using AuthorizeService.Repository;
using AuthorizeService.Services;
using Moq;
using Xunit;

namespace AuthoriseService.UnitTests.Services
{
    public class AuthorisationServiceTests
    {
        [Fact]
        public void test()
        {
            var merchantRepository = new Mock<MerchantRepository>();
            var authorisationService = new AuthorisationService(merchantRepository.Object);
        }
    }
}