using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IInvoiceService
    {
        Task<PaginatedResponse<Invoice>> GetInvoicesPaginatedAsync(int companyId, int projectId, PaginationQuery query);
        Task<InvoiceDetailDto?> GetInvoiceDetailAsync(int companyId, int id);
        Task<Invoice> CreateInvoiceAsync(int companyId, CreateInvoiceDto dto);
        Task<Invoice?> UpdateInvoiceAsync(int companyId, int id, UpdateInvoiceDto dto);
        Task<bool> UpdateInvoiceStatusAsync(int companyId, int id, string status);
        Task<bool> DeleteInvoiceAsync(int companyId, int id);
        Task<Payment> RecordPaymentAsync(int companyId, int invoiceId, RecordPaymentDto dto);
        Task<ReceivableSummaryDto> GetReceivableSummaryAsync(int companyId);
    }
}
