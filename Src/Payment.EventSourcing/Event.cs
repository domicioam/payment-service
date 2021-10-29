using System;

namespace Payment.EventSourcing
{
    public class Event : Message
    {
        public DateTime When { get; private set; }

        public Event()
            : base()
        {
            When = DateTime.UtcNow;
            Name = GetType().Name;
        }
    }
}
