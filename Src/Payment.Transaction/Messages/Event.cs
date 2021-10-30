using System;

namespace Payment.EventSourcing
{
    public class Event : Message
    {
        public DateTime When { get; }
        public int Version { get; }

        public Event(int version)
            : base()
        {
            When = DateTime.UtcNow;
            Name = GetType().Name;
            Version = version;
        }
    }
}
