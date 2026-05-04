using System.Data;
using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Users (CompanyId, Name, Email, PasswordHash, Role, CreatedAt)
                VALUES (@CompanyId, @Name, @Email, @PasswordHash, @Role, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            user.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Email = @Email LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetUserByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Users WHERE CompanyId = @CompanyId ORDER BY CreatedAt ASC;";
            return await connection.QueryAsync<User>(sql, new { CompanyId = companyId });
        }

        public async Task<bool> UpdateUserRoleAsync(int companyId, int id, string role)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Users SET Role = @Role WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Role = role, Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<bool> DeleteUserAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Users WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }
    }
}
