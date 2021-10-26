using System;

namespace AuthorizeService.Services
{
    public interface CanValidateMerchant
    {
        bool IsMerchantValid(Guid merchant);
    }
}