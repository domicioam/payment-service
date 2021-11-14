using System;
using System.Threading.Tasks;
using AuthorizeService.Entities;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Payment.EventSourcing;
using Payment.EventSourcing.Config;

namespace AuthorizeService.Repository
{
    public class MerchantRepository
    {
        private readonly Database _database;
        
        protected MerchantRepository() {}

        public MerchantRepository(IOptions<Database> databaseConfig)
        {
            _database = databaseConfig.Value;
        }
        
        public virtual async Task<Merchant> GetByIdAsync(Guid id)
        {
            await using var connection = new NpgsqlConnection($"User ID=postgres;Password=password;Host={_database.PaymentAuthorise.Host};Port=5432;Database=authorise;");
            var merchant = await connection.QuerySingleAsync<Merchant>($"SELECT * FROM \"Merchant\" WHERE \"Id\" = \'{id}\'");
            return merchant;
        }
    }
}