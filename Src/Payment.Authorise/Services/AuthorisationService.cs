using System;

namespace AuthorizeService.Services
{
    public class AuthorisationService : CanValidateMerchant, CanValidateCreditCard
    {
        public bool IsMerchantValid(Guid merchant)
        {
            throw new NotImplementedException();
        }

        public bool IsCreditCardValid(CreditCard creditCard)
        {
            throw new NotImplementedException();
        }
    }
}