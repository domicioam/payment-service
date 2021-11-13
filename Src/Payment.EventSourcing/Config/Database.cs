namespace Payment.EventSourcing.Config
{
    public class Database
    {
        public PaymentAuthorise PaymentAuthorise { get; set; }
        public EventStore EventStore { get; set; }
    }
}