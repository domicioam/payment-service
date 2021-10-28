using System.Threading;
using System.Threading.Tasks;
using AuthorizeService.Application;
using PaymentGatewayWorker.EventSourcing;
using MediatR;

namespace AuthorizeService.EventSourcing
{
    public class AuthoriseEventStore : INotificationHandler<AuthorisationCreated>,
        INotificationHandler<AuthorisationRejected>
    {
        private readonly EventStore _eventStore;

        public AuthoriseEventStore(EventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        public async Task Handle(AuthorisationCreated notification, CancellationToken cancellationToken)
        {
            await _eventStore.SaveAsync(notification);
        }

        public async Task Handle(AuthorisationRejected notification, CancellationToken cancellationToken)
        {
            await _eventStore.SaveAsync(notification);
        }
    }
}