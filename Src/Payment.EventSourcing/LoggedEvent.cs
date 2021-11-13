using System;
using Dapper.Contrib.Extensions;

namespace Payment.EventSourcing
{
    [Table("LoggedEvent")]
    public class LoggedEvent
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public Guid AggregateId { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Version { get; set; }
    }
}
