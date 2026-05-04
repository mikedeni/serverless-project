using System.Data;
using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _context;

        public ProjectRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompanyIdAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Projects WHERE CompanyId = @CompanyId ORDER BY CreatedAt DESC;";
            return await connection.QueryAsync<Project>(sql, new { CompanyId = companyId });
        }

        public async Task<Project?> GetProjectByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT 
                    p.*, 
                    (
                        (SELECT COALESCE(SUM(Amount), 0) FROM Expenses WHERE ProjectId = p.Id AND CompanyId = p.CompanyId) +
                        (SELECT COALESCE(SUM(Qty * UnitPrice), 0) FROM MaterialTransactions WHERE ProjectId = p.Id AND CompanyId = p.CompanyId AND Type = 'purchase_in') +
                        (SELECT COALESCE(SUM(sp.Amount), 0) FROM SubcontractorPayments sp JOIN SubcontractorContracts sc ON sp.ContractId = sc.Id WHERE sc.ProjectId = p.Id AND sc.CompanyId = p.CompanyId) +
                        (SELECT COALESCE(SUM(w.DailyWage), 0) FROM Attendances a JOIN Workers w ON a.WorkerId = w.Id WHERE a.ProjectId = p.Id AND a.CompanyId = p.CompanyId)
                    ) AS TotalSpent
                FROM Projects p 
                WHERE p.CompanyId = @CompanyId AND p.Id = @Id 
                LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Project>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateProjectAsync(Project project)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Projects (CompanyId, ProjectName, StartDate, EndDate, Budget, Status, ImageUrl, CreatedAt)
                VALUES (@CompanyId, @ProjectName, @StartDate, @EndDate, @Budget, @Status, @ImageUrl, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            project.CreatedAt = DateTime.UtcNow;
            
            return await connection.ExecuteScalarAsync<int>(sql, project);
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Projects 
                SET ProjectName = @ProjectName, 
                    StartDate = @StartDate, 
                    EndDate = @EndDate, 
                    Budget = @Budget, 
                    Status = @Status,
                    ImageUrl = @ImageUrl
                WHERE Id = @Id AND CompanyId = @CompanyId;";
            
            var affectedRows = await connection.ExecuteAsync(sql, project);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteProjectAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Projects WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<(IEnumerable<Project> Items, int TotalCount)> GetProjectsPaginatedAsync(
            int companyId, int offset, int pageSize, string? search, string? status)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE p.CompanyId = @CompanyId";
            if (!string.IsNullOrWhiteSpace(search))
                whereClause += " AND p.ProjectName LIKE @Search";
            if (!string.IsNullOrWhiteSpace(status))
                whereClause += " AND p.Status = @Status";

            var countSql = $"SELECT COUNT(*) FROM Projects p {whereClause};";
            var dataSql = $@"
                SELECT 
                    p.*, 
                    COALESCE(costs.TotalSpent, 0) AS TotalSpent
                FROM Projects p
                LEFT JOIN (
                    SELECT ProjectId, SUM(Amount) AS TotalSpent FROM (
                        SELECT ProjectId, Amount FROM Expenses WHERE CompanyId = @CompanyId
                        UNION ALL
                        SELECT ProjectId, (Qty * UnitPrice) FROM MaterialTransactions WHERE CompanyId = @CompanyId AND Type = 'purchase_in'
                        UNION ALL
                        SELECT sc.ProjectId, sp.Amount FROM SubcontractorPayments sp JOIN SubcontractorContracts sc ON sp.ContractId = sc.Id WHERE sp.CompanyId = @CompanyId
                        UNION ALL
                        SELECT a.ProjectId, w.DailyWage FROM Attendances a JOIN Workers w ON a.WorkerId = w.Id WHERE a.CompanyId = @CompanyId
                    ) combined_costs GROUP BY ProjectId
                ) costs ON p.Id = costs.ProjectId
                {whereClause} 
                ORDER BY p.CreatedAt DESC 
                LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                Search = $"%{search}%",
                Status = status,
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Project>(dataSql, parameters);

            return (items, totalCount);
        }
        
        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Projects SET ImageUrl = @ImageUrl WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { ImageUrl = imageUrl, Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }
    }
}
