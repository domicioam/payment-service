using System;

namespace Payment.EventSourcing
{
    public class Message
    {
        public Guid AggregateId { get; protected set; }
        public string Name { get; protected set; }
        
        protected bool Equals(Message other)
        {
            return AggregateId.Equals(other.AggregateId) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AggregateId, Name);
        }
    }
}
