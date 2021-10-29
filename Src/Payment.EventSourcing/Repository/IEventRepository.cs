using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment.EventSourcing.Repository
{
    public interface IEventRepository
    {
        Task SaveAsync(LoggedEvent @event);
        Task<IList<LoggedEvent>> AllAsync(Guid aggregateId);
    }
}