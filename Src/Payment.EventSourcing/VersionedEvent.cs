using System.Transactions;

namespace Payment.EventSourcing
{
    public class VersionedEvent : Event
    {
        public int Version { get; }

        public VersionedEvent(int version): base()
        {
            Version = version;
        }
    }
}