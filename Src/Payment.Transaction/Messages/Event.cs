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
        
        protected bool Equals(Event other)
        {
            return When.Equals(other.When) && Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Event) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(When, Version);
        }
    }
}
