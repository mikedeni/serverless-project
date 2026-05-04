using System.Data;
using Dapper;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCompanyAsync(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Companies (Name, CreatedAt)
                VALUES (@Name, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            return await connection.ExecuteScalarAsync<int>(sql, new { Name = name, CreatedAt = DateTime.UtcNow });
        }
    }
}
