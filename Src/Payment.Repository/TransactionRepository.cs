using System;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Payment.EventSourcing.Messages;
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
        public virtual async Task<Transaction.Aggregates.Transaction> GetByIdAsync(Guid aggregateId)
        {
            var events = await _eventRepository.AllAsync(aggregateId);
            var transaction = new Transaction.Aggregates.Transaction(_mediator);

            foreach (var @event in events)
            {
                switch (@event.Action)
                {
                    case nameof(AuthorisationCreated):
                        var authorisationCreated = JsonSerializer.Deserialize<AuthorisationCreated>(@event.Data);
                        transaction.Apply(authorisationCreated);
                        break;
                    case nameof(CaptureExecuted):
                        var captureExecuted = JsonSerializer.Deserialize<CaptureExecuted>(@event.Data);
                        transaction.Apply(captureExecuted);
                        break;
                    case nameof(CaptureRejected):
                        var captureRejected = JsonSerializer.Deserialize<CaptureRejected>(@event.Data);
                        transaction.Apply(captureRejected);
                        break;
                    case nameof(RefundExecuted) :
                        var refundExecuted = JsonSerializer.Deserialize<RefundExecuted>(@event.Data);
                        transaction.Apply(refundExecuted);
                        break;
                    case nameof(RefundRejected) :
                        var refundRejected = JsonSerializer.Deserialize<RefundRejected>(@event.Data);
                        transaction.Apply(refundRejected);
                        break;
                    case nameof(CaptureCompleted) :
                        var captureCompleted = JsonSerializer.Deserialize<CaptureCompleted>(@event.Data);
                        transaction.Apply(captureCompleted);
                        break;
                    case nameof(RefundCompleted) :
                        var refundCompleted = JsonSerializer.Deserialize<RefundCompleted>(@event.Data);
                        transaction.Apply(refundCompleted);
                        break;
                    case nameof(TransactionVoided) :
                        var transactionVoided = JsonSerializer.Deserialize<TransactionVoided>(@event.Data);
                        transaction.Apply(transactionVoided);
                        break;
                    case nameof(VoidRejected):
                        var voidRejected = JsonSerializer.Deserialize<VoidRejected>(@event.Data);
                        transaction.Apply(voidRejected);
                        break;
                }
            }

            return transaction;
        }
    }
}