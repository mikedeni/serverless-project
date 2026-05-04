using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IQuotationRepository
    {
        Task<IEnumerable<Quotation>> GetQuotationsByProjectAsync(int companyId, int projectId);
        Task<(IEnumerable<Quotation> Items, int TotalCount)> GetQuotationsPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, string? status);
        Task<Quotation?> GetQuotationByIdAsync(int companyId, int id);
        Task<int> CreateQuotationAsync(Quotation quotation);
        Task<bool> UpdateQuotationAsync(Quotation quotation);
        Task<bool> UpdateQuotationStatusAsync(int companyId, int id, string status);
        Task<bool> DeleteQuotationAsync(int companyId, int id);

        // Items
        Task<IEnumerable<QuotationItem>> GetQuotationItemsAsync(int quotationId);
        Task<int> AddQuotationItemAsync(QuotationItem item);
        Task<bool> DeleteQuotationItemAsync(int quotationId, int itemId);
        Task DeleteAllQuotationItemsAsync(int quotationId);

        // Counter
        Task<int> GetQuotationCountByCompanyAsync(int companyId);
    }
}
