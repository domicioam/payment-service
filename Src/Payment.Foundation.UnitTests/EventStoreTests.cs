using System;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Payment.Foundation.EventSourcing;
using PaymentGatewayWorker.EventSourcing;
using Xunit;

namespace Payment.Foundation.UnitTests
{
    public class EventStoreTests
    {
        [Fact]
        public void Should_save_event()
        {
            var eventRepository = new Mock<IEventRepository>();
            var logger = new Mock<ILogger<EventStore>>();
            var eventStore = new EventStore(eventRepository.Object, logger.Object);
            var @event = new Event();
            eventStore.SaveAsync(@event);
            eventRepository.Verify(e => e.SaveAsync(It.IsAny<LoggedEvent>()), Times.Once);
        }
    }
}