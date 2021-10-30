using System;
using System.Transactions;
using MediatR;
using Moq;
using Payment.EventSourcing.Messages;
using Xunit;
using TransactionStatus = Payment.Transaction.Aggregates.TransactionStatus;

namespace Payment.Transaction.UnitTests.Aggregates
{
    public class TransactionTests
    {
        private readonly Mock<IMediator> _mediator;

        public TransactionTests()
        {
            _mediator = new Mock<IMediator>();
        }

        
        [Fact]
        public void Should_initialize_transaction()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            transaction.Apply(authorisationCreated);
            
            Assert.Equal(merchantId, transaction.MerchantId);
            Assert.Equal(authorisationId, transaction.Id);
            Assert.Equal(amount, transaction.Amount);
        }

        [Fact]
        public void Should_decrease_amount_available_when_capture_event_is_applied()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            transaction.Apply(authorisationCreated);
            var capturedAmount = 10m;
            var eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            transaction.Apply(captureExecuted);

            Assert.Equal(amount - capturedAmount, transaction.Amount);
            Assert.Equal(eventVersion, transaction.Version);
        }

        [Fact]
        public void Should_refund_full_amount_after_one_capture()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            transaction.Apply(authorisationCreated);
            var capturedAmount = 10m;
            var eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            transaction.Apply(captureExecuted);
            var refundExecuted = new RefundExecuted(authorisationId, amount, 3);
            transaction.Apply(refundExecuted);
            
            Assert.Equal(transaction.Amount, amount);
            Assert.Equal(TransactionStatus.Refunded, transaction.Status);
        }
        
        [Fact]
        public void Should_not_refund_when_refund_was_rejected()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            transaction.Apply(authorisationCreated);
            var capturedAmount = 10m;
            var eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            transaction.Apply(captureExecuted);
            var rejected = new RefundRejected(authorisationId, 3);
            transaction.Apply(rejected);
            
            Assert.Equal(amount - capturedAmount, transaction.Amount);
            Assert.Equal(TransactionStatus.Active, transaction.Status);
        }
    }
}