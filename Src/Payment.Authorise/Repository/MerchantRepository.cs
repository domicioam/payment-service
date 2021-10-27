using System;
using System.Threading.Tasks;
using AuthorizeService.Entities;
using Dapper;
using Npgsql;

namespace AuthorizeService.Repository
{
    public class MerchantRepository
    {
        public virtual async Task<Merchant> GetByIdAsync(Guid id)
        {
            await using var connection = new NpgsqlConnection("User ID=postgres;Password=password;Host=localhost;Port=5432;Database=authorise;");
            var merchant = await connection.QuerySingleAsync<Merchant>($"SELECT * FROM Merchant WHERE ID = {id}");
            return merchant;
        }
    }
}