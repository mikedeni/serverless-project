using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface ISubcontractorRepository
    {
        Task<(IEnumerable<Subcontractor> Items, int TotalCount)> GetSubcontractorsPaginatedAsync(int companyId, int offset, int pageSize, string? search, string? status);
        Task<Subcontractor?> GetSubcontractorByIdAsync(int companyId, int id);
        Task<int> CreateSubcontractorAsync(Subcontractor sub);
        Task<bool> UpdateSubcontractorAsync(Subcontractor sub);
        Task<bool> DeleteSubcontractorAsync(int companyId, int id);

        // Contracts
        Task<IEnumerable<SubcontractorContract>> GetContractsBySubAsync(int companyId, int subId);
        Task<IEnumerable<SubcontractorContract>> GetContractsByProjectAsync(int companyId, int projectId);
        Task<SubcontractorContract?> GetContractByIdAsync(int companyId, int id);
        Task<int> CreateContractAsync(SubcontractorContract contract);
        Task<bool> UpdateContractStatusAsync(int companyId, int id, string status);
        Task<bool> UpdateContractPaidAmountAsync(int contractId, decimal newPaidAmount);

        // Payments
        Task<IEnumerable<SubcontractorPayment>> GetPaymentsByContractAsync(int contractId);
        Task<decimal> GetTotalPaidByContractAsync(int contractId);
        Task<int> CreateSubPaymentAsync(SubcontractorPayment payment);

        // Summary
        Task<(int TotalContracts, decimal TotalValue, decimal TotalPaid)> GetPayableSummaryAsync(int companyId);
    }
}
