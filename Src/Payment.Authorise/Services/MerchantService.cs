using System;

namespace AuthorizeService.Services
{
    public interface MerchantService
    {
        bool IsMerchantValid(Guid merchant);
    }
}