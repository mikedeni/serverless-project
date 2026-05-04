using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IQuotationService
    {
        Task<PaginatedResponse<Quotation>> GetQuotationsPaginatedAsync(int companyId, int projectId, PaginationQuery query);
        Task<QuotationDetailDto?> GetQuotationDetailAsync(int companyId, int id);
        Task<Quotation> CreateQuotationAsync(int companyId, CreateQuotationDto dto);
        Task<Quotation?> UpdateQuotationAsync(int companyId, int id, UpdateQuotationDto dto);
        Task<bool> UpdateQuotationStatusAsync(int companyId, int id, string status);
        Task<bool> DeleteQuotationAsync(int companyId, int id);
        Task<QuotationItem> AddQuotationItemAsync(int companyId, int quotationId, QuotationItemDto dto);
        Task<bool> DeleteQuotationItemAsync(int companyId, int quotationId, int itemId);
    }
}
