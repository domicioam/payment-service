using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Capture.Repository;
using Payment.EventSourcing.Messages;

namespace Payment.Void.Services
{
    public class VoidService
    {
        public VoidService(TransactionRepository transactionRepository, ILogger<VoidService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        private readonly TransactionRepository _transactionRepository;
        private readonly ILogger<VoidService> _logger;

        public async Task Void(VoidCommand voidCommand)
        {
            _logger.LogInformation($"[Void] Void started for authorisation: {voidCommand.AggregateId}");
            
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(voidCommand.AggregateId);
                transaction.Process(voidCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[Void] Error when trying to void authorisation id: {voidCommand.AggregateId}.");
            }
        }
    }
}
