using System;
using MediatR;
using Payment.Foundation.EventSourcing;

namespace AuthorizeService.Application
{
    public class AuthorisationCreated : Event, INotification
    {
        public AuthorisationCreated(Guid MerchantId, Guid AuthorisationId)
        {
            this.MerchantId = MerchantId;
            this.AuthorisationId = AuthorisationId;
        }

        public Guid MerchantId { get; init; }
        public Guid AuthorisationId { get; init; }

        public void Deconstruct(out Guid MerchantId, out Guid AuthorisationId)
        {
            MerchantId = this.MerchantId;
            AuthorisationId = this.AuthorisationId;
        }
    }
}