using System.Data;
using MySqlConnector;

namespace ConstructionSaaS.Api.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new Exception("DefaultConnection string is missing.");
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}
