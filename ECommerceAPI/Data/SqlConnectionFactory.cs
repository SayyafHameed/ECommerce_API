using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ECommerceAPI.Data
{
    public class PostgreSqlConnectionFactory : DbContext
    {
        private readonly IConfiguration _configuration;
        public PostgreSqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public NpgsqlConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        } 
    }
}
