using System;
using System.Threading;
using MediatR;
using Moq;
using Payment.EventSourcing.Messages;
using Payment.Transaction.Aggregates;
using Xunit;

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
            const decimal capturedAmount = 10m;
            const int eventVersion = 3;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            
            transaction.Apply(authorisationCreated);
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
            const decimal capturedAmount = 10m;
            const int eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            var refundExecuted = new RefundExecuted(authorisationId, amount, 3);
            
            transaction.Apply(captureExecuted);
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
            const decimal capturedAmount = 10m;
            const int eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            var rejected = new RefundRejected(authorisationId, 3);
            
            transaction.Apply(captureExecuted);
            transaction.Apply(rejected);

            Assert.Equal(amount - capturedAmount, transaction.Amount);
            Assert.Equal(TransactionStatus.Active, transaction.Status);
        }

        [Fact]
        public void Should_process_capture()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal captureAmount = 10m;
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureCommand = new CaptureCommand(authorisationId, captureAmount);

            transaction.Apply(authorisationCreated);
            transaction.Process(captureCommand);

            var expected = new CaptureExecuted(authorisationId, captureAmount, 2);
            _mediator.Verify(
                m => m.Publish(
                    It.Is<CaptureExecuted>(env =>
                        env.Amount == expected.Amount && env.Version == expected.Version &&
                        env.AggregateId == expected.AggregateId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Should_throw_when_try_to_capture_in_refunded_status()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var refundExecuted = new RefundExecuted(authorisationId, amount, 3);
            var captureCommand = new CaptureCommand(authorisationId, 10m);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            transaction.Apply(refundExecuted);
            
            Assert.Throws<InvalidOperationException>(() => transaction.Process(captureCommand));
            Assert.Equal(TransactionStatus.Refunded, transaction.Status);
            Assert.Equal(3, transaction.Version);
        }

        [Fact]
        public void Should_complete_when_two_captures_with_total_amount()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var firstCaptureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var captureCommand = new CaptureCommand(authorisationId, 10m);

            transaction.Apply(authorisationCreated);
            transaction.Apply(firstCaptureExecuted);
            transaction.Process(captureCommand);

            var expected = new CaptureCompleted(authorisationId, 3);
            _mediator.Verify(
                m => m.Publish(
                    It.Is<CaptureCompleted>(env =>
                        env.Version == expected.Version &&
                        env.AggregateId == expected.AggregateId), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public void Should_status_be_completed_after_capture_completed()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var firstCaptureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var captureCompleted = new CaptureCompleted(authorisationId, 3);

            transaction.Apply(authorisationCreated);
            transaction.Apply(firstCaptureExecuted);
            transaction.Apply(captureCompleted);

            Assert.Equal(TransactionStatus.Completed, transaction.Status);
            Assert.Equal(3, transaction.Version);
        }
        
        [Fact]
        public void Should_throw_when_try_to_capture_in_completed_status()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var captureCompleted = new CaptureCompleted(authorisationId, 3);

            var captureCommand = new CaptureCommand(authorisationId, 10m);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            transaction.Apply(captureCompleted);
            
            Assert.Throws<InvalidOperationException>(() => transaction.Process(captureCommand));
            Assert.Equal(TransactionStatus.Completed, transaction.Status);
            Assert.Equal(3, transaction.Version);
        }
    }
}