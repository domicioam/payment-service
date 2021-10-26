namespace AuthorizeService.Services
{
    public interface CreditCardService
    {
        bool IsCreditCardValid(CreditCard creditCard);
    }
}