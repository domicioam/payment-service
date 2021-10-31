using System;
using System.Threading.Tasks;
using AuthorizeService.Factories;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.EventSourcing.Messages;

namespace AuthorizeService.Services
{
    public class AuthoriseService
    {
        private readonly ILogger<AuthoriseService> _logger;
        private readonly IMediator _mediator;
        private readonly AuthorisationFactory _authorisationFactory;
        private readonly CanValidateCreditCard _cardService;
        private readonly CanValidateMerchant _canValidateMerchant;

        public AuthoriseService(ILogger<AuthoriseService> logger, IMediator mediator, 
            AuthorisationFactory authorisationFactory, CanValidateCreditCard cardService, CanValidateMerchant canValidateMerchant)
        {
            _logger = logger;
            _mediator = mediator;
            _authorisationFactory = authorisationFactory;
            _cardService = cardService;
            _canValidateMerchant = canValidateMerchant;
        }
        
        public async Task AuthoriseAsync(AuthorisationCommand authoriseCommand)
        {
            var (transactionId, merchantId, creditCard, currency, amount) = authoriseCommand;
            try
            {
                if (_cardService.IsCreditCardValid(creditCard, DateTime.Today) && await _canValidateMerchant.IsMerchantValidAsync(merchantId))
                {
                    var authorisation = _authorisationFactory.CreateAuthorisation(transactionId, merchantId, creditCard, currency, amount);
                    await _mediator.Publish(new AuthorisationCreated(merchantId, authorisation.Id, amount));
                    _logger.LogInformation($"[Authorise] Authorisation created with id: {authorisation.Id}");
                    return;
                }

                _logger.LogWarning($"[Authorise] Authorisation rejected for merchant with id: {merchantId}");
                await _mediator.Publish(new AuthorisationRejected(merchantId, creditCard.Number));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[Authorise] Error when trying to authorise command for merchant with id: {merchantId}");
            }
        }
    }
}