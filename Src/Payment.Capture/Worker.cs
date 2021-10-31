using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Capture.Services;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;

namespace Payment.Capture
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly CaptureService _captureService;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer, CaptureService captureService)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
            _captureService = captureService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartListeningForCaptureRequests();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task StartListeningForCaptureRequests()
        {
            _rabbitMqConsumer.MessageReceived += ProcessCaptureRequest;
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.Capture);
        }

        private async void ProcessCaptureRequest(string message)
        {
            try
            {
                var captureCommand = JsonSerializer.Deserialize<CaptureCommand>(message);
                await _captureService.Capture(captureCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message.", message);
            }
        }
    }
}