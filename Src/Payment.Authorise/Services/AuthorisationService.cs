using System;
using System.Threading.Tasks;
using AuthorizeService.Repository;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

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

        public bool IsCreditCardValid(CreditCard creditCard, DateTime validUntil)
        {
            //TODO: validation with card company
            return creditCard.IsValid(validUntil);
        }
    }
}