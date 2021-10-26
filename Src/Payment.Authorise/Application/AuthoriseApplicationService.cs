using System;
using AuthorizeService.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthorizeService.Application
{
    public class AuthoriseApplicationService
    {
        private readonly ILogger<AuthoriseApplicationService> _logger;
        private readonly IMediator _mediator;
        private readonly AuthorisationService _authorisationService;
        private readonly CreditCardService _cardService;
        private readonly MerchantService _merchantService;

        public AuthoriseApplicationService(ILogger<AuthoriseApplicationService> logger, IMediator mediator, 
            AuthorisationService authorisationService, CreditCardService cardService, MerchantService merchantService)
        {
            _logger = logger;
            _mediator = mediator;
            _authorisationService = authorisationService;
            _cardService = cardService;
            _merchantService = merchantService;
        }
        
        public void Authorise(AuthorisationCommand authoriseCommand)
        {
            if (_cardService.IsCreditCardValid(authoriseCommand.CreditCard) && _merchantService.IsMerchantValid(authoriseCommand.MerchantId))
            {
                var authorisation = _authorisationService.CreateAuthorisation(authoriseCommand.MerchantId, authoriseCommand.CreditCard,
                    authoriseCommand.Currency, authoriseCommand.Amount);
                _mediator.Send(new AuthorisationCreated(authoriseCommand.MerchantId, authorisation.Id));
                _logger.LogInformation($"[Authorise] Authorisation created with id: {authorisation.Id}");
                return;
            }
            
            _logger.LogWarning($"[Authorise] Authorisation rejected for merchant with id: {authoriseCommand.MerchantId}");
            _mediator.Send(new AuthorisationRejected(authoriseCommand.MerchantId, authoriseCommand.CreditCard.Number));
        }
    }
}