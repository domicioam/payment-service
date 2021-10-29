using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;

namespace Payment.EventSourcing
{
    /// <summary>
    /// Silly implementation of an event store.
    /// </summary>
    public class EventStore : IEventStore, INotificationHandler<AuthorisationCreated>, INotificationHandler<AuthorisationRejected>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventStore> _logger;
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public EventStore(IEventRepository eventRepository, ILogger<EventStore> logger, RabbitMqPublisher rabbitMqPublisher)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task SaveAsync<T>(T @event) where T : Event
        {
            var loggedEvent = new LoggedEvent()
            {
                Action = @event.Name,
                AggregateId = @event.AggregateId,
                Data = JsonSerializer.Serialize(@event, @event.GetType())
            };

            try
            {
                await _eventRepository.SaveAsync(loggedEvent);
                string message = JsonSerializer.Serialize(@event);
                const string storedEventsQueue = "stored-events";
                await _rabbitMqPublisher.SendMessageAsync(message, storedEventsQueue);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error while trying to save logged event.");
            }
        }

        public async Task Handle(AuthorisationCreated notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(AuthorisationRejected notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }
    }
}
