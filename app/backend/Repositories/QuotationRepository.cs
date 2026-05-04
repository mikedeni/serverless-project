using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly DapperContext _context;

        public QuotationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quotation>> GetQuotationsByProjectAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Quotations WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId ORDER BY CreatedAt DESC;";
            return await connection.QueryAsync<Quotation>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<(IEnumerable<Quotation> Items, int TotalCount)> GetQuotationsPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, string? status)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId";
            if (!string.IsNullOrWhiteSpace(status))
                whereClause += " AND Status = @Status";

            var countSql = $"SELECT COUNT(*) FROM Quotations {whereClause};";
            var dataSql = $"SELECT * FROM Quotations {whereClause} ORDER BY CreatedAt DESC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                ProjectId = projectId,
                Status = status,
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Quotation>(dataSql, parameters);

            return (items, totalCount);
        }

        public async Task<Quotation?> GetQuotationByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Quotations WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Quotation>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateQuotationAsync(Quotation quotation)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Quotations (ProjectId, CompanyId, QuotationNumber, ClientName, ClientAddress, ClientPhone, 
                    Status, MarkupPercent, Discount, TaxPercent, Note, ValidUntil, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @QuotationNumber, @ClientName, @ClientAddress, @ClientPhone,
                    @Status, @MarkupPercent, @Discount, @TaxPercent, @Note, @ValidUntil, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            quotation.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, quotation);
        }

        public async Task<bool> UpdateQuotationAsync(Quotation quotation)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Quotations 
                SET ClientName = @ClientName, ClientAddress = @ClientAddress, ClientPhone = @ClientPhone,
                    MarkupPercent = @MarkupPercent, Discount = @Discount, TaxPercent = @TaxPercent,
                    Note = @Note, ValidUntil = @ValidUntil
                WHERE Id = @Id AND CompanyId = @CompanyId;";

            var affectedRows = await connection.ExecuteAsync(sql, quotation);
            return affectedRows > 0;
        }

        public async Task<bool> UpdateQuotationStatusAsync(int companyId, int id, string status)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Quotations SET Status = @Status WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId, Status = status });
            return affectedRows > 0;
        }

        public async Task<bool> DeleteQuotationAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Quotations WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        // --- Items ---

        public async Task<IEnumerable<QuotationItem>> GetQuotationItemsAsync(int quotationId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM QuotationItems WHERE QuotationId = @QuotationId ORDER BY ItemOrder ASC;";
            return await connection.QueryAsync<QuotationItem>(sql, new { QuotationId = quotationId });
        }

        public async Task<int> AddQuotationItemAsync(QuotationItem item)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO QuotationItems (QuotationId, ItemOrder, Description, Qty, Unit, UnitPrice, CreatedAt)
                VALUES (@QuotationId, @ItemOrder, @Description, @Qty, @Unit, @UnitPrice, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            item.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, item);
        }

        public async Task<bool> DeleteQuotationItemAsync(int quotationId, int itemId)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM QuotationItems WHERE Id = @ItemId AND QuotationId = @QuotationId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { ItemId = itemId, QuotationId = quotationId });
            return affectedRows > 0;
        }

        public async Task DeleteAllQuotationItemsAsync(int quotationId)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM QuotationItems WHERE QuotationId = @QuotationId;";
            await connection.ExecuteAsync(sql, new { QuotationId = quotationId });
        }

        public async Task<int> GetQuotationCountByCompanyAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Quotations WHERE CompanyId = @CompanyId;";
            return await connection.ExecuteScalarAsync<int>(sql, new { CompanyId = companyId });
        }
    }
}
