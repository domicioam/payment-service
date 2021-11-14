using System;
using MediatR;
using Payment.EventSourcing.Messages;

namespace Payment.Transaction.Aggregates
{
    //TODO: Refactor to state pattern
    public enum TransactionStatus
    {
        Active,
        Voided,
        Refunded,
        Completed,
        NotStarted
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
        public decimal AvailableAmount { get; private set; }
        public decimal InitialAmount { get; private set; }
        public TransactionStatus Status { get; private set; }

        public void Process(CaptureCommand captureCommand)
        {
            var (aggregateId, amount) = captureCommand;
            if (aggregateId != Id || Status is TransactionStatus.Voided or TransactionStatus.Refunded or TransactionStatus.Completed)
                throw new InvalidOperationException($"[Process] Invalid operation for Capture with id: {aggregateId}.");

            if (amount > AvailableAmount)
            {
                _mediator.Publish(new CaptureRejected(Id, Version + 1));
            }
            else if (amount == AvailableAmount)
            {
                _mediator.Publish(new CaptureCompleted(Id, Version + 1));
            }
            else
            {
                _mediator.Publish(new CaptureExecuted(Id, amount, Version + 1));
            }
        }

        public void Process(RefundCommand refundCommand)
        {
            var (aggregateId, amount) = refundCommand;
            if (aggregateId != Id || Status is TransactionStatus.Voided or TransactionStatus.Refunded)
                throw new InvalidOperationException($"[Process] Invalid operation for Refund with id: {aggregateId}.");

            //TODO: Refactor to state pattern

            if (Status == TransactionStatus.Active)
            {
                if (AvailableAmount - amount == 0)
                {
                    _mediator.Publish(new RefundCompleted(Id, Version + 1));
                }
                else if (InitialAmount - amount == 0)
                {
                    _mediator.Publish(new RefundRejected(Id, Version + 1));
                }
                else
                {
                    _mediator.Publish(new RefundExecuted(Id, amount, Version + 1));
                }
            }
            else if (Status == TransactionStatus.Completed)
            {
                if (InitialAmount - amount == 0)
                {
                    _mediator.Publish(new RefundCompleted(Id, Version + 1));
                }
                else
                {
                    _mediator.Publish(new RefundExecuted(Id, amount, Version + 1));
                }
            }
        }

        public void Process(VoidCommand voidCommand)
        {
            if (Id != voidCommand.AggregateId)
                throw new InvalidOperationException($"[Process] Invalid operation for Void with id: {voidCommand.AggregateId}.");
            
            if (Status == TransactionStatus.NotStarted)
            {
                _mediator.Publish(new TransactionVoided(Id, Version + 1));
            }

            _mediator.Publish(new VoidRejected(Id, Version + 1));
        }

        public void Apply(AuthorisationCreated authorisationCreated)
        {
            Id = authorisationCreated.AggregateId;
            MerchantId = authorisationCreated.MerchantId;
            InitialAmount = authorisationCreated.Amount;
            AvailableAmount = authorisationCreated.Amount;
            Status = TransactionStatus.NotStarted;
            Version = 1;
        }

        public void Apply(CaptureExecuted captureExecuted)
        {
            Status = TransactionStatus.Active;
            AvailableAmount -= captureExecuted.Amount;
            Version = captureExecuted.Version;
        }

        public void Apply(CaptureRejected captureRejected)
        {
            Version = captureRejected.Version;
        }

        public void Apply(RefundExecuted refundExecuted)
        {
            //TODO: Refactor to state pattern

            if (Status == TransactionStatus.Completed)
            {
                AvailableAmount = refundExecuted.Amount;
                Status = TransactionStatus.Active;
                Version = refundExecuted.Version;
            }
            else
            {
                AvailableAmount += refundExecuted.Amount;
                Version = refundExecuted.Version;
                Status = TransactionStatus.Active;
            }
        }

        public void Apply(RefundRejected refundRejected)
        {
            Version = refundRejected.Version;
        }

        public void Apply(CaptureCompleted captureCompleted)
        {
            AvailableAmount = 0;
            Version = captureCompleted.Version;
            Status = TransactionStatus.Completed;
        }

        public void Apply(RefundCompleted refundCompleted)
        {
            AvailableAmount = InitialAmount;
            Version = refundCompleted.Version;
            Status = TransactionStatus.Refunded;
        }
        
        public void Apply(TransactionVoided transactionVoided)
        {
            AvailableAmount = 0;
            Version = transactionVoided.Version;
            Status = TransactionStatus.Voided;
        }
        
        public void Apply(VoidRejected voidRejected)
        {
            Version = voidRejected.Version;
        }
    }
}