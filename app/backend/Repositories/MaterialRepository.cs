using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly DapperContext _context;

        public MaterialRepository(DapperContext context)
        {
            _context = context;
        }

        // --- Materials ---

        public async Task<(IEnumerable<Material> Items, int TotalCount)> GetMaterialsPaginatedAsync(
            int companyId, int offset, int pageSize, string? search)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE CompanyId = @CompanyId";
            if (!string.IsNullOrWhiteSpace(search))
                whereClause += " AND Name LIKE @Search";

            var countSql = $"SELECT COUNT(*) FROM Materials {whereClause};";
            var dataSql = $"SELECT * FROM Materials {whereClause} ORDER BY Name ASC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                Search = $"%{search}%",
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Material>(dataSql, parameters);

            return (items, totalCount);
        }

        public async Task<Material?> GetMaterialByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Materials WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Material>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateMaterialAsync(Material material)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Materials (CompanyId, Name, Unit, CurrentStock, MinStock, LastPrice, ImageUrl, CreatedAt)
                VALUES (@CompanyId, @Name, @Unit, @CurrentStock, @MinStock, @LastPrice, @ImageUrl, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            material.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, material);
        }

        public async Task<bool> UpdateMaterialAsync(Material material)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Materials 
                SET Name = @Name, Unit = @Unit, MinStock = @MinStock, ImageUrl = @ImageUrl
                WHERE Id = @Id AND CompanyId = @CompanyId;";

            var affectedRows = await connection.ExecuteAsync(sql, material);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteMaterialAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Materials WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Materials WHERE CompanyId = @CompanyId AND CurrentStock <= MinStock AND MinStock > 0 ORDER BY Name ASC;";
            return await connection.QueryAsync<Material>(sql, new { CompanyId = companyId });
        }

        public async Task<bool> UpdateMaterialStockAsync(int materialId, decimal newStock, decimal? lastPrice)
        {
            using var connection = _context.CreateConnection();
            var sql = lastPrice.HasValue
                ? "UPDATE Materials SET CurrentStock = @NewStock, LastPrice = @LastPrice WHERE Id = @MaterialId;"
                : "UPDATE Materials SET CurrentStock = @NewStock WHERE Id = @MaterialId;";

            var affectedRows = await connection.ExecuteAsync(sql, new { MaterialId = materialId, NewStock = newStock, LastPrice = lastPrice });
            return affectedRows > 0;
        }

        // --- Transactions ---

        public async Task<int> CreateTransactionAsync(MaterialTransaction transaction)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO MaterialTransactions (MaterialId, ProjectId, CompanyId, Type, Qty, UnitPrice, Note, Date, CreatedAt)
                VALUES (@MaterialId, @ProjectId, @CompanyId, @Type, @Qty, @UnitPrice, @Note, @Date, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            transaction.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, transaction);
        }

        public async Task<IEnumerable<MaterialTransaction>> GetTransactionsByMaterialAsync(int companyId, int materialId, int limit = 50)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT mt.*, m.Name AS MaterialName, p.ProjectName
                FROM MaterialTransactions mt
                JOIN Materials m ON mt.MaterialId = m.Id
                LEFT JOIN Projects p ON mt.ProjectId = p.Id
                WHERE mt.CompanyId = @CompanyId AND mt.MaterialId = @MaterialId
                ORDER BY mt.Date DESC, mt.CreatedAt DESC
                LIMIT @Limit;";

            return await connection.QueryAsync<MaterialTransaction>(sql, new { CompanyId = companyId, MaterialId = materialId, Limit = limit });
        }

        public async Task<IEnumerable<MaterialTransaction>> GetTransactionsByProjectAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT mt.*, m.Name AS MaterialName, p.ProjectName
                FROM MaterialTransactions mt
                JOIN Materials m ON mt.MaterialId = m.Id
                LEFT JOIN Projects p ON mt.ProjectId = p.Id
                WHERE mt.CompanyId = @CompanyId AND mt.ProjectId = @ProjectId
                ORDER BY mt.Date DESC;";

            return await connection.QueryAsync<MaterialTransaction>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Materials SET ImageUrl = @ImageUrl WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { ImageUrl = imageUrl, Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }
    }
}
