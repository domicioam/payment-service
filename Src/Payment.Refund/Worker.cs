using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;
using Payment.Refund.Application;

namespace Payment.Refund
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly RefundService _refundService;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer, RefundService refundService)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
            _refundService = refundService;
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
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.Refund);
        }

        private async void ProcessRefundRequest(string message)
        {
            try
            {
                var refundCommand = JsonSerializer.Deserialize<RefundCommand>(message);
                await _refundService.Refund(refundCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message.", message);
            }
        }
    }
}