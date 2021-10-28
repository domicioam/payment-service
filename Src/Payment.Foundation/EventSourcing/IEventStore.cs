using System.Threading.Tasks;

namespace Payment.Foundation.EventSourcing
{
    public interface IEventStore
    {
        Task SaveAsync<T>(T theEvent) where T : Event;
    }
}