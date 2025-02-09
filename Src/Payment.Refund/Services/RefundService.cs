﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Capture.Repository;
using Payment.EventSourcing.Messages;

namespace Payment.Refund.Application
{
    public class RefundService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly ILogger<RefundService> _logger;
        
        public RefundService(TransactionRepository transactionRepository, ILogger<RefundService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }
        
        public async Task Refund(RefundCommand refundCommand)
        {
            var (authorisationId, amount) = refundCommand;
            _logger.LogInformation($"[Refund] Refund started for authorisation: {authorisationId}");
            
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(refundCommand.AggregateId);
                transaction.Process(refundCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[Refund] Error when trying to refund for authorisation id: {authorisationId}.");
            }
        }
    }
}