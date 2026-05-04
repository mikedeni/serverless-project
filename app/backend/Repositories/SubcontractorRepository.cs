using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class SubcontractorRepository : ISubcontractorRepository
    {
        private readonly DapperContext _context;
        public SubcontractorRepository(DapperContext context) { _context = context; }

        public async Task<(IEnumerable<Subcontractor> Items, int TotalCount)> GetSubcontractorsPaginatedAsync(int companyId, int offset, int pageSize, string? search, string? status)
        {
            using var connection = _context.CreateConnection();
            var where = "WHERE CompanyId = @CompanyId";
            if (!string.IsNullOrWhiteSpace(search)) where += " AND Name LIKE CONCAT('%', @Search, '%')";
            if (!string.IsNullOrWhiteSpace(status)) where += " AND Status = @Status";

            var count = await connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM Subcontractors {where};", new { CompanyId = companyId, Search = search, Status = status });
            var items = await connection.QueryAsync<Subcontractor>($"SELECT * FROM Subcontractors {where} ORDER BY CreatedAt DESC LIMIT @PageSize OFFSET @Offset;",
                new { CompanyId = companyId, Search = search, Status = status, PageSize = pageSize, Offset = offset });
            return (items, count);
        }

        public async Task<Subcontractor?> GetSubcontractorByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Subcontractor>("SELECT * FROM Subcontractors WHERE CompanyId = @CompanyId AND Id = @Id;", new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateSubcontractorAsync(Subcontractor sub)
        {
            using var connection = _context.CreateConnection();
            sub.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Subcontractors (CompanyId, Name, Specialty, Phone, Email, Status, CreatedAt)
                VALUES (@CompanyId, @Name, @Specialty, @Phone, @Email, @Status, @CreatedAt); SELECT LAST_INSERT_ID();", sub);
        }

        public async Task<bool> UpdateSubcontractorAsync(Subcontractor sub)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(
                @"UPDATE Subcontractors SET Name=@Name, Specialty=@Specialty, Phone=@Phone, Email=@Email, Status=@Status WHERE Id=@Id AND CompanyId=@CompanyId;", sub) > 0;
        }

        public async Task<bool> DeleteSubcontractorAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Subcontractors WHERE Id=@Id AND CompanyId=@CompanyId;", new { Id = id, CompanyId = companyId }) > 0;
        }

        // Contracts
        public async Task<IEnumerable<SubcontractorContract>> GetContractsBySubAsync(int companyId, int subId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SubcontractorContract>(
                @"SELECT c.*, s.Name as SubcontractorName, p.ProjectName FROM SubcontractorContracts c
                LEFT JOIN Subcontractors s ON c.SubcontractorId = s.Id
                LEFT JOIN Projects p ON c.ProjectId = p.Id
                WHERE c.CompanyId = @CompanyId AND c.SubcontractorId = @SubId ORDER BY c.CreatedAt DESC;",
                new { CompanyId = companyId, SubId = subId });
        }

        public async Task<IEnumerable<SubcontractorContract>> GetContractsByProjectAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SubcontractorContract>(
                @"SELECT c.*, s.Name as SubcontractorName, p.ProjectName FROM SubcontractorContracts c
                LEFT JOIN Subcontractors s ON c.SubcontractorId = s.Id
                LEFT JOIN Projects p ON c.ProjectId = p.Id
                WHERE c.CompanyId = @CompanyId AND c.ProjectId = @ProjectId ORDER BY c.CreatedAt DESC;",
                new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<SubcontractorContract?> GetContractByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SubcontractorContract>(
                @"SELECT c.*, s.Name as SubcontractorName, p.ProjectName FROM SubcontractorContracts c
                LEFT JOIN Subcontractors s ON c.SubcontractorId = s.Id
                LEFT JOIN Projects p ON c.ProjectId = p.Id
                WHERE c.CompanyId = @CompanyId AND c.Id = @Id;",
                new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateContractAsync(SubcontractorContract contract)
        {
            using var connection = _context.CreateConnection();
            contract.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO SubcontractorContracts (SubcontractorId, ProjectId, CompanyId, Scope, ContractAmount, PaidAmount, Status, StartDate, EndDate, CreatedAt)
                VALUES (@SubcontractorId, @ProjectId, @CompanyId, @Scope, @ContractAmount, 0, @Status, @StartDate, @EndDate, @CreatedAt); SELECT LAST_INSERT_ID();", contract);
        }

        public async Task<bool> UpdateContractStatusAsync(int companyId, int id, string status)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE SubcontractorContracts SET Status=@Status WHERE Id=@Id AND CompanyId=@CompanyId;",
                new { Id = id, CompanyId = companyId, Status = status }) > 0;
        }

        public async Task<bool> UpdateContractPaidAmountAsync(int contractId, decimal newPaidAmount)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE SubcontractorContracts SET PaidAmount=@PaidAmount WHERE Id=@Id;",
                new { Id = contractId, PaidAmount = newPaidAmount }) > 0;
        }

        // Payments
        public async Task<IEnumerable<SubcontractorPayment>> GetPaymentsByContractAsync(int contractId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SubcontractorPayment>("SELECT * FROM SubcontractorPayments WHERE ContractId=@ContractId ORDER BY PaymentDate DESC;",
                new { ContractId = contractId });
        }

        public async Task<decimal> GetTotalPaidByContractAsync(int contractId)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Amount),0) FROM SubcontractorPayments WHERE ContractId=@ContractId;",
                new { ContractId = contractId });
        }

        public async Task<int> CreateSubPaymentAsync(SubcontractorPayment payment)
        {
            using var connection = _context.CreateConnection();
            payment.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO SubcontractorPayments (ContractId, CompanyId, Amount, PaymentDate, Note, CreatedAt)
                VALUES (@ContractId, @CompanyId, @Amount, @PaymentDate, @Note, @CreatedAt); SELECT LAST_INSERT_ID();", payment);
        }

        // Summary
        public async Task<(int TotalContracts, decimal TotalValue, decimal TotalPaid)> GetPayableSummaryAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT COUNT(*) as TotalContracts, COALESCE(SUM(ContractAmount),0) as TotalValue, COALESCE(SUM(PaidAmount),0) as TotalPaid
                FROM SubcontractorContracts WHERE CompanyId=@CompanyId AND Status != 'cancelled';";
            var result = await connection.QueryFirstAsync<dynamic>(sql, new { CompanyId = companyId });
            return ((int)result.TotalContracts, (decimal)result.TotalValue, (decimal)result.TotalPaid);
        }
    }
}
