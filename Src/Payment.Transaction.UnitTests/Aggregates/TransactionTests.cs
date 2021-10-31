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
            Assert.Equal(amount, transaction.AvailableAmount);
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

            Assert.Equal(amount - capturedAmount, transaction.AvailableAmount);
            Assert.Equal(eventVersion, transaction.Version);
        }
        
        [Fact]
        public void Should_refund_after_one_capture()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            const decimal capturedAmount = 10m;
            const int eventVersion = 3;
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, eventVersion);
            var refundCompleted = new RefundCompleted(authorisationId, 3);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            transaction.Apply(refundCompleted);

            Assert.Equal(amount, transaction.AvailableAmount);
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

            Assert.Equal(amount - capturedAmount, transaction.AvailableAmount);
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
            var refundCompleted = new RefundCompleted(authorisationId, 3);
            var captureCommand = new CaptureCommand(authorisationId, 10m);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            transaction.Apply(refundCompleted);
            
            Assert.Throws<InvalidOperationException>(() => transaction.Process(captureCommand));
            Assert.Equal(TransactionStatus.Refunded, transaction.Status);
            Assert.Equal(3, transaction.Version);
        }
        
        [Fact]
        public void Should_reject_refund_with_amount_above_available()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var refundCommand = new RefundCommand(authorisationId, amount);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            
            transaction.Process(refundCommand);
            
            _mediator.Verify(
                m => m.Publish(
                    It.Is<RefundRejected>(r => 
                        r.Version == 3), It.IsAny<CancellationToken>()),Times.Once);
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

        [Fact]
        public void Should_refund_in_two_parcels()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            const decimal capturedAmount = 10m;
            
            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, capturedAmount, 2);
            var captureCompleted = new CaptureCompleted(authorisationId, 3);
            var refundExecuted = new RefundExecuted(authorisationId, 10m, 4);
            
            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);
            transaction.Apply(captureCompleted);
            transaction.Apply(refundExecuted);
            
            Assert.Equal(10m, transaction.AvailableAmount);
            Assert.Equal(TransactionStatus.Active, transaction.Status);
            Assert.Equal(4, transaction.Version);

            var refundCommand = new RefundCommand(authorisationId, 10m);
            transaction.Process(refundCommand);
            
            _mediator.Verify(m =>
                m.Publish(
                    It.Is<RefundExecuted>(n => 
                        n.Amount == 10m && n.Version == 5), It.IsAny<CancellationToken>()), Times.Never);
            
            _mediator.Verify(m =>
                m.Publish(
                    It.Is<RefundCompleted>(n => 
                        n.Version == 5), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Should_send_refund_completed_after_process_command()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureCompleted = new CaptureCompleted(authorisationId, 2);

            transaction.Apply(authorisationCreated);
            transaction.Apply(captureCompleted);

            var refundCommand = new RefundCommand(authorisationId, amount);

            transaction.Process(refundCommand);

            _mediator.Verify(m => m.Publish(It.Is<RefundCompleted>(r => r.Version == 3), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Should_send_refund_executed_after_partial_refund()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureCompleted = new CaptureCompleted(authorisationId, 2);

            transaction.Apply(authorisationCreated);
            transaction.Apply(captureCompleted);

            var partialAmount = 10m;
            var refundCommand = new RefundCommand(authorisationId, partialAmount);

            transaction.Process(refundCommand);

            _mediator.Verify(m => m.Publish(It.Is<RefundExecuted>(r => r.Version == 3 && r.Amount == 10m), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Should_execute_partial_capture_when_refund_is_less_than_captured_amount()
        {
            var transaction = new Transaction.Aggregates.Transaction(_mediator.Object);
            var merchantId = Guid.NewGuid();
            var authorisationId = Guid.NewGuid();
            const decimal amount = 20m;
            var partialAmount = 10m;
            var refundAmount = 5m;

            var authorisationCreated = new AuthorisationCreated(merchantId, authorisationId, amount);
            var captureExecuted = new CaptureExecuted(authorisationId, partialAmount, 2);

            transaction.Apply(authorisationCreated);
            transaction.Apply(captureExecuted);

            var refundCommand = new RefundCommand(authorisationId, refundAmount);

            transaction.Process(refundCommand);

            _mediator.Verify(m => m.Publish(It.Is<RefundExecuted>(r => r.Version == 3 && r.Amount == refundAmount), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}