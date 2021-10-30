using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Capture.Repository;
using Payment.EventSourcing.Messages;

namespace Payment.Capture.Application
{
    public class CaptureApplicationService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly ILogger<CaptureApplicationService> _logger;

        public CaptureApplicationService(TransactionRepository transactionRepository, ILogger<CaptureApplicationService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task Capture(CaptureCommand captureCommand)
        {
            var (authorisationId, amount) = captureCommand;
            _logger.LogInformation($"[Capture] Capture started for authorisation: {authorisationId}");
            
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(captureCommand.AggregateId);
                transaction.Process(captureCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[Capture] Error when trying to capture for authorisation id: {authorisationId}.");
            }
        }
    }
}