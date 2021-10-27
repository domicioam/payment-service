using System;
using System.Threading.Tasks;

namespace AuthorizeService.Services
{
    public interface CanValidateMerchant
    {
        Task<bool> IsMerchantValidAsync(Guid merchant);
    }
}