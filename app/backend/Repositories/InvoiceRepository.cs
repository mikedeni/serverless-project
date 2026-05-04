using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DapperContext _context;

        public InvoiceRepository(DapperContext context)
        {
            _context = context;
        }

        // --- Invoice CRUD ---

        public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetInvoicesPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, string? status)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId";
            if (!string.IsNullOrWhiteSpace(status))
                whereClause += " AND Status = @Status";

            var countSql = $"SELECT COUNT(*) FROM Invoices {whereClause};";
            var dataSql = $"SELECT * FROM Invoices {whereClause} ORDER BY CreatedAt DESC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                ProjectId = projectId,
                Status = status,
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Invoice>(dataSql, parameters);

            return (items, totalCount);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Invoices WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Invoice>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateInvoiceAsync(Invoice invoice)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Invoices (ProjectId, CompanyId, InvoiceNumber, ClientName, Description,
                    Amount, TaxPercent, TaxAmount, TotalAmount, DueDate, Status, MilestoneLabel, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @InvoiceNumber, @ClientName, @Description,
                    @Amount, @TaxPercent, @TaxAmount, @TotalAmount, @DueDate, @Status, @MilestoneLabel, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            invoice.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, invoice);
        }

        public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Invoices
                SET ClientName = @ClientName, Description = @Description,
                    Amount = @Amount, TaxPercent = @TaxPercent, TaxAmount = @TaxAmount,
                    TotalAmount = @TotalAmount, DueDate = @DueDate, MilestoneLabel = @MilestoneLabel
                WHERE Id = @Id AND CompanyId = @CompanyId;";

            var affectedRows = await connection.ExecuteAsync(sql, invoice);
            return affectedRows > 0;
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int companyId, int id, string status)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Invoices SET Status = @Status WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId, Status = status });
            return affectedRows > 0;
        }

        public async Task<bool> DeleteInvoiceAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Invoices WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        // --- Payments ---

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Payments WHERE InvoiceId = @InvoiceId ORDER BY PaymentDate DESC;";
            return await connection.QueryAsync<Payment>(sql, new { InvoiceId = invoiceId });
        }

        public async Task<decimal> GetTotalPaidByInvoiceAsync(int invoiceId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE InvoiceId = @InvoiceId;";
            return await connection.ExecuteScalarAsync<decimal>(sql, new { InvoiceId = invoiceId });
        }

        public async Task<int> CreatePaymentAsync(Payment payment)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Payments (InvoiceId, CompanyId, Amount, PaymentDate, Method, Note, CreatedAt)
                VALUES (@InvoiceId, @CompanyId, @Amount, @PaymentDate, @Method, @Note, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            payment.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, payment);
        }

        // --- Counter & Summary ---

        public async Task<int> GetInvoiceCountByCompanyAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Invoices WHERE CompanyId = @CompanyId;";
            return await connection.ExecuteScalarAsync<int>(sql, new { CompanyId = companyId });
        }

        public async Task<(int TotalInvoices, decimal TotalInvoiced, decimal TotalPaid, int OverdueCount, decimal OverdueAmount)> GetReceivableSummaryAsync(int companyId)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    COUNT(*) AS TotalInvoices,
                    COALESCE(SUM(TotalAmount), 0) AS TotalInvoiced
                FROM Invoices 
                WHERE CompanyId = @CompanyId AND Status != 'cancelled';";
            var invoiceSummary = await connection.QueryFirstAsync<dynamic>(sql, new { CompanyId = companyId });

            var paidSql = @"
                SELECT COALESCE(SUM(p.Amount), 0)
                FROM Payments p
                INNER JOIN Invoices i ON p.InvoiceId = i.Id
                WHERE i.CompanyId = @CompanyId;";
            var totalPaid = await connection.ExecuteScalarAsync<decimal>(paidSql, new { CompanyId = companyId });

            var overdueSql = @"
                SELECT 
                    COUNT(*) AS OverdueCount,
                    COALESCE(SUM(TotalAmount), 0) AS OverdueAmount
                FROM Invoices 
                WHERE CompanyId = @CompanyId AND Status = 'overdue';";
            var overdue = await connection.QueryFirstAsync<dynamic>(overdueSql, new { CompanyId = companyId });

            return (
                (int)invoiceSummary.TotalInvoices,
                (decimal)invoiceSummary.TotalInvoiced,
                totalPaid,
                (int)overdue.OverdueCount,
                (decimal)overdue.OverdueAmount
            );
        }
    }
}
