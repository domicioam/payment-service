﻿using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;
using Payment.EventSourcing.Repository;

namespace Payment.EventSourcing
{
    /// <summary>
    /// Silly implementation of an event store.
    /// </summary>
    public class EventStore : IEventStore, INotificationHandler<AuthorisationCreated>, 
        INotificationHandler<AuthorisationRejected>,
        INotificationHandler<CaptureCompleted>,
        INotificationHandler<CaptureExecuted>,
        INotificationHandler<CaptureRejected>,
        INotificationHandler<RefundCompleted>,
        INotificationHandler<RefundExecuted>,
        INotificationHandler<RefundRejected>,
        INotificationHandler<TransactionVoided>,
        INotificationHandler<VoidRejected>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventStore> _logger;
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public EventStore(IEventRepository eventRepository, ILogger<EventStore> logger, RabbitMqPublisher rabbitMqPublisher)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        /// <summary>
        /// Saves the event and performs an event publish atomically.
        /// </summary>
        /// <param name="event">Event to be stored.</param>
        /// <typeparam name="T">Event type.</typeparam>
        public async Task SaveAsync<T>(T @event) where T : Event
        {
            var loggedEvent = new LoggedEvent()
            {
                Action = @event.Name,
                AggregateId = @event.AggregateId,
                Data = JsonSerializer.Serialize(@event, @event.GetType()),
                Version = @event.Version
            };

            try
            {
                await _eventRepository.SaveAsync(loggedEvent);
                string message = JsonSerializer.Serialize(@event);
                await _rabbitMqPublisher.SendMessageAsync(message, Queues.StoredEvents);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error while trying to save logged event.");
            }
        }

        public async Task Handle(AuthorisationCreated notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(AuthorisationRejected notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(CaptureCompleted notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(CaptureExecuted notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(CaptureRejected notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(RefundCompleted notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(RefundExecuted notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(RefundRejected notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(TransactionVoided notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }

        public async Task Handle(VoidRejected notification, CancellationToken cancellationToken)
        {
            await SaveAsync<Event>(notification);
        }
    }
}
