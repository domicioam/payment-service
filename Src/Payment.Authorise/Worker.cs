using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AuthorizeService.Application;
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
        private readonly AuthoriseApplicationService _authoriseApplicationService;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer rabbitMqConsumer, AuthoriseApplicationService authoriseApplicationService)
        {
            _logger = logger;
            _rabbitMqConsumer = rabbitMqConsumer;
            _authoriseApplicationService = authoriseApplicationService;
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
                var authoriseDto = JsonSerializer.Deserialize<AuthorisationCommand>(message);
                await _authoriseApplicationService.AuthoriseAsync(authoriseDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message.", message);
            }
        }
    }
}