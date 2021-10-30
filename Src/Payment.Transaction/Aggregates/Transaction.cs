using System;
using MediatR;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace Payment.Capture.Aggregates
{
    public enum TransactionStatus
    {
        Active,
        Voided
    }
    
    public class Transaction
    {
        //TODO: Encapsulate in domain event publisher
        private readonly IMediator _mediator;

        public Transaction(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public Guid Id { get; private set; }
        public Guid MerchantId { get; private set; }
        public int Version { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionStatus Status { get; private set; }

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
            var (aggregateId, amount) = captureCommand;
            if (aggregateId != Id || Status == TransactionStatus.Voided)
                throw new InvalidOperationException($"[Process] Invalid operation for Capture with id: {aggregateId}.");

            if (amount > Amount)
            {
                _mediator.Publish(new CaptureRejected(Id, Version + 1));
                return;
            }

            _mediator.Publish(new CaptureExecuted(Id, amount, Version + 1));
        }
        
        public void Process(RefundCommand refundCommand)
        {
            var (aggregateId, amount) = refundCommand;
            if (aggregateId != Id || Status == TransactionStatus.Voided)
                throw new InvalidOperationException($"[Process] Invalid operation for Refund with id: {aggregateId}.");

            if (amount > Amount)
            {
                _mediator.Publish(new RefundRejected(Id, Version + 1));
                return;
            }

            _mediator.Publish(new RefundExecuted(Id, amount, Version + 1));
        }
        
        private void Apply(AuthorisationCreated authorisationCreated)
        {
            Id = authorisationCreated.AggregateId;
            MerchantId = authorisationCreated.MerchantId;
            Amount = authorisationCreated.Amount;
            Status = TransactionStatus.Active;
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

        private void Apply(RefundExecuted refundExecuted)
        {
            Version++;
            Status = TransactionStatus.Voided;
        }
        
        private void Apply(RefundRejected refundRejected)
        {
            Version++;
        }
    }
}