using System;
using MediatR;
using Payment.EventSourcing;
using Payment.EventSourcing.Messages;

namespace Payment.Transaction.Aggregates
{
    public enum TransactionStatus
    {
        Active,
        Voided,
        Refunded,
        Completed
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
                case RefundExecuted refundExecuted:
                    Apply(refundExecuted);
                    break;
                case RefundRejected refundRejected:
                    Apply(refundRejected);
                    break;
                case CaptureCompleted captureCompleted:
                    Apply(captureCompleted);
                    break;
            }
        }
        
        public void Process(CaptureCommand captureCommand)
        {
            var (aggregateId, amount) = captureCommand;
            if (aggregateId != Id || Status is TransactionStatus.Voided or TransactionStatus.Refunded or TransactionStatus.Completed)
                throw new InvalidOperationException($"[Process] Invalid operation for Capture with id: {aggregateId}.");

            if (amount > Amount)
            {
                _mediator.Publish(new CaptureRejected(Id, Version + 1));
                return;
            }

            if (amount == Amount)
            {
                _mediator.Publish(new CaptureCompleted(Id, Version + 1));
                return;
            }

            _mediator.Publish(new CaptureExecuted(Id, amount, Version + 1));
        }
        
        public void Process(RefundCommand refundCommand)
        {
            var (aggregateId, amount) = refundCommand;
            if (aggregateId != Id || Status is TransactionStatus.Voided or TransactionStatus.Refunded)
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
            Version = 1;
        }

        private void Apply(CaptureExecuted captureExecuted)
        {
            Amount -= captureExecuted.Amount;
            Version = captureExecuted.Version;
        }

        private void Apply(CaptureRejected captureRejected)
        {
            Version = captureRejected.Version;
        }

        private void Apply(RefundExecuted refundExecuted)
        {
            Amount = refundExecuted.Amount;
            Version = refundExecuted.Version;
            Status = TransactionStatus.Refunded;
        }
        
        private void Apply(RefundRejected refundRejected)
        {
            Version = refundRejected.Version;
        }
        
        private void Apply(CaptureCompleted captureCompleted)
        {
            Amount = 0;
            Version = captureCompleted.Version;
            Status = TransactionStatus.Completed;
        }
    }
}