using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;

namespace Payment.Void
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
            await StartListeningForVoidRequests();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task StartListeningForVoidRequests()
        {
            _rabbitMqConsumer.MessageReceived += ProcessVoidRequest;
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.StoredEvents);
        }

        private void ProcessVoidRequest(string message)
        {
            // filter void events
            throw new NotImplementedException();
        }
    }
}