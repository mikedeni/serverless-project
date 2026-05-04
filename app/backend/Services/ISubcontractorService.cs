using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface ISubcontractorService
    {
        Task<PaginatedResponse<Subcontractor>> GetSubcontractorsPaginatedAsync(int companyId, PaginationQuery query);
        Task<SubcontractorDetailDto?> GetSubcontractorDetailAsync(int companyId, int id);
        Task<Subcontractor> CreateSubcontractorAsync(int companyId, CreateSubcontractorDto dto);
        Task<Subcontractor?> UpdateSubcontractorAsync(int companyId, int id, UpdateSubcontractorDto dto);
        Task<bool> DeleteSubcontractorAsync(int companyId, int id);

        Task<SubcontractorContract> CreateContractAsync(int companyId, CreateContractDto dto);
        Task<IEnumerable<SubcontractorContract>> GetContractsByProjectAsync(int companyId, int projectId);
        Task<bool> UpdateContractStatusAsync(int companyId, int id, string status);
        Task<SubcontractorPayment> RecordSubPaymentAsync(int companyId, int contractId, RecordSubPaymentDto dto);
        Task<PayableSummaryDto> GetPayableSummaryAsync(int companyId);
    }
}
