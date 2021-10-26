using AuthorizeService.Entities;

namespace AuthorizeService.Services
{
    public interface MerchantValidator
    {
        public bool IsMerchantValid(Merchant merchant);
    }
}