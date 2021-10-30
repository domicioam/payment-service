using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;

namespace Payment.Refund
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _rabbitMqConsumer;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartListeningForRefundRequests();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task StartListeningForRefundRequests()
        {
            _rabbitMqConsumer.MessageReceived += ProcessRefundRequest;
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.StoredEvents);
        }

        private void ProcessRefundRequest(string message)
        {
            // filter refund events
            throw new NotImplementedException();
        }
    }
}