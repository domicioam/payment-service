using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Payment.EventSourcing.Repository
{
    public class EventRepository : IEventRepository
    {
        const string connectionString = "User ID=postgres;Password=password;Host=localhost;Port=5433;Database=eventstore;";

        public async Task SaveAsync(LoggedEvent @event)
        {
            @event.TimeStamp = DateTime.Now; //TODO: make utc again
            await using var connection = new NpgsqlConnection(connectionString);
            
            string sql = $"INSERT INTO \"LoggedEvent\" (\"Action\", \"AggregateId\", \"Data\", \"Version\", \"TimeStamp\") " +
                         $"VALUES (@Action, @AggregateId, @Data, @Version, @TimeStamp);";

            await connection.ExecuteAsync(sql, @event);
        }

        public async Task<IList<LoggedEvent>> AllAsync(Guid aggregateId)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            var events = await connection.QueryAsync<LoggedEvent>($"SELECT * FROM \"LoggedEvent\" WHERE \"Id\" = {aggregateId}");
            return events.ToList();
        }
    }
}
