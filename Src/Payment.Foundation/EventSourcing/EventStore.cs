using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Payment.Foundation.EventSourcing;

namespace PaymentGatewayWorker.EventSourcing
{
    public class EventStore : IEventStore
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventStore> _logger;
        
        public EventStore(IEventRepository eventRepository, ILogger<EventStore> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
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
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error while trying to save logged event.");
            }
        }
    }
}
