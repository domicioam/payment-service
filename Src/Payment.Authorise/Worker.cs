using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AuthorizeService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;

namespace AuthorizeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly AuthoriseService _authoriseService;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer, AuthoriseService authoriseService)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
            _authoriseService = authoriseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartListeningForAuthorisationRequests();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task StartListeningForAuthorisationRequests()
        {
            _rabbitMqConsumer.MessageReceived += ProcessAuthoriseRequest;
            await _rabbitMqConsumer.StartListeningForRequestsAsync(Queues.Authorise);
        }

        private async void ProcessAuthoriseRequest(string message)
        {
            try
            {
                _logger.LogInformation("Message received.");
                var authoriseDto = JsonSerializer.Deserialize<AuthorisationCommand>(message);
                await _authoriseService.AuthoriseAsync(authoriseDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message.", message);
            }
        }
    }
}