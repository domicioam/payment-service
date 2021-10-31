using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;
using Payment.Void.Services;

namespace Payment.Void
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly VoidService _voidService;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer, VoidService voidService)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
            _voidService = voidService;
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
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.Void);
        }

        private async void ProcessVoidRequest(string message)
        {
            try
            {
                var voidCommand = JsonSerializer.Deserialize<VoidCommand>(message);
                await _voidService.Void(voidCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message.", message);
            }
        }
    }
}