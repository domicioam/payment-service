using System;
using MediatR;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace Payment.Capture.Aggregates
{
    public class Transaction
    {
        private readonly IMediator _mediator;

        public Transaction(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public Guid Id { get; private set; }
        public Guid MerchantId { get; private set; }
        public int Version { get; private set; }
        public decimal Amount { get; private set; }

        public void Apply(Event @event)
        {
            switch (@event)
            {
                case AuthorisationCreated authorisationCreated:
                    Apply(authorisationCreated);
                    break;
                case CaptureExecuted captureExecuted:
                    Apply(captureExecuted);
                    break;
                case CaptureRejected captureRejected:
                    Apply(captureRejected);
                    break;
            }
        }
        
        public void Process(CaptureCommand captureCommand)
        {
            if (captureCommand.AggregateId != Id)
                throw new InvalidOperationException($"[Process] The aggregate id provided is invalid: {captureCommand.AggregateId}.");

            if (captureCommand.Amount > Amount)
            {
                _mediator.Publish(new CaptureRejected(Id, Version + 1));
                return;
            }

            _mediator.Publish(new CaptureExecuted(Id, captureCommand.Amount, Version + 1));
        }
        
        private void Apply(AuthorisationCreated authorisationCreated)
        {
            Id = authorisationCreated.AggregateId;
            MerchantId = authorisationCreated.MerchantId;
            Amount = authorisationCreated.Amount;
            Version++;
        }

        private void Apply(CaptureExecuted captureExecuted)
        {
            Amount -= captureExecuted.Amount;
            Version++;
        }

        private void Apply(CaptureRejected captureRejected)
        {
            Version++;
        }
    }
}