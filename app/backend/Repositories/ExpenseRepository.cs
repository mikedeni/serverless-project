using System.Data;
using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DapperContext _context;

        public ExpenseRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateExpenseAsync(Expense expense)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Expenses (ProjectId, CompanyId, Amount, Category, Date, Note, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @Amount, @Category, @Date, @Note, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            expense.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, expense);
        }

        public async Task<Expense?> GetExpenseByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Expenses WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Expense>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<IEnumerable<Expense>> GetExpensesByProjectIdAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Expenses WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId ORDER BY Date DESC;";
            return await connection.QueryAsync<Expense>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<decimal> GetTotalExpensesByProjectIdAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT (
                    (SELECT COALESCE(SUM(Amount), 0) FROM Expenses WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId) +
                    (SELECT COALESCE(SUM(Qty * UnitPrice), 0) FROM MaterialTransactions WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId AND Type = 'purchase_in') +
                    (SELECT COALESCE(SUM(sp.Amount), 0) FROM SubcontractorPayments sp JOIN SubcontractorContracts sc ON sp.ContractId = sc.Id WHERE sc.CompanyId = @CompanyId AND sc.ProjectId = @ProjectId) +
                    (SELECT COALESCE(SUM(w.DailyWage), 0) FROM Attendances a JOIN Workers w ON a.WorkerId = w.Id WHERE a.CompanyId = @CompanyId AND a.ProjectId = @ProjectId)
                ) AS TotalSpent;";
            return await connection.ExecuteScalarAsync<decimal>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<bool> UpdateExpenseAsync(Expense expense)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Expenses 
                SET Amount = @Amount, 
                    Category = @Category, 
                    Date = @Date, 
                    Note = @Note
                WHERE Id = @Id AND CompanyId = @CompanyId;";
            
            var affectedRows = await connection.ExecuteAsync(sql, expense);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteExpenseAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Expenses WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<(IEnumerable<Expense> Items, int TotalCount)> GetExpensesPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, string? category)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId";
            if (!string.IsNullOrWhiteSpace(category))
                whereClause += " AND Category = @Category";

            var countSql = $"SELECT COUNT(*) FROM Expenses {whereClause};";
            var dataSql = $"SELECT * FROM Expenses {whereClause} ORDER BY Date DESC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                ProjectId = projectId,
                Category = category,
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Expense>(dataSql, parameters);

            return (items, totalCount);
        }
    }
}
