using System;

namespace Payment.Foundation.EventSourcing
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
