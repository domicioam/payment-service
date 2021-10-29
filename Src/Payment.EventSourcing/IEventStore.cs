using System.Threading.Tasks;

namespace Payment.EventSourcing
{
    public interface IEventStore
    {
        Task SaveAsync<T>(T theEvent) where T : Event;
    }
}