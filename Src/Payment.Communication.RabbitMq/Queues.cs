namespace Payment.Communication.RabbitMq
{
    public static class Queues
    {
        public const string StoredEvents = "stored-events";
        public const string Authorise = "authorise";
        public const string Capture = "capture";
        public const string Refund = "refund";
        public const string Void = "void";
    }
}