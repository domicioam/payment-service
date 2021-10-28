using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Dapper.Contrib.Extensions;
using Payment.Foundation.EventSourcing;

namespace PaymentGatewayWorker.EventSourcing
{
    class EventRepository : IEventRepository
    {
        public async Task SaveAsync(LoggedEvent @event)
        {
            @event.TimeStamp = DateTime.UtcNow;
            await using var connection = new NpgsqlConnection("User ID=postgres;Password=password;Host=localhost;Port=5432;Database=authorise;");
            connection.Insert(@event);

        }

        public async Task<IList<LoggedEvent>> AllAsync(Guid aggregateId)
        {
            await using var connection = new NpgsqlConnection("User ID=postgres;Password=password;Host=localhost;Port=5432;Database=authorise;");
            var events = await connection.QueryAsync<LoggedEvent>($"SELECT * FROM LoggedEvent WHERE ID = {aggregateId}");
            return events.ToList();
        }

        // Necessary for mocking
        protected EventRepository()
        {

        }
    }
}
