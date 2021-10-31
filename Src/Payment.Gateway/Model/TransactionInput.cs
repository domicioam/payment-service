using System;

namespace Payment.Gateway.Model
{
    public record TransactionRequest(Action Action, Guid MerchantId, decimal Amount);
}