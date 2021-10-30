using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Payment.Capture.Aggregates;
using Payment.EventSourcing;
using Payment.EventSourcing.Repository;

namespace Payment.Capture.Repository
{
    public class TransactionRepository
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMediator _mediator;

        public TransactionRepository(IEventRepository eventRepository, IMediator mediator)
        {
            _eventRepository = eventRepository;
            _mediator = mediator;
        }
        public virtual async Task<Transaction> GetByIdAsync(Guid aggregateId)
        {
            var events = await _eventRepository.AllAsync(aggregateId);
            var deserializedEvents = events.Select(e => JsonSerializer.Deserialize<Event>(e.Data));
            
            var transaction = new Transaction(_mediator);

            foreach (var @event in deserializedEvents)
            {
                transaction.Apply(@event);
            }

            return transaction;
        }
    }
}