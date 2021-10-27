using System;
using System.Threading.Tasks;
using AuthorizeService.Repository;

namespace AuthorizeService.Services
{
    public class AuthorisationService : CanValidateMerchant, CanValidateCreditCard
    {
        private readonly MerchantRepository _merchantRepository;

        public AuthorisationService(MerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }
        
        public async Task<bool> IsMerchantValidAsync(Guid merchantId)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId);
            return merchant.IsActive;
        }

        public bool IsCreditCardValid(CreditCard creditCard)
        {
            throw new NotImplementedException();
        }
    }
}