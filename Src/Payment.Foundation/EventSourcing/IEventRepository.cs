using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentGatewayWorker.EventSourcing;

namespace Payment.Foundation.EventSourcing
{
    public interface IEventRepository
    {
        Task SaveAsync(LoggedEvent @event);
        Task<IList<LoggedEvent>> AllAsync(Guid aggregateId);
    }
}