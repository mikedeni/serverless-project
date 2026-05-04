using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IInvoiceRepository
    {
        // Invoice CRUD
        Task<(IEnumerable<Invoice> Items, int TotalCount)> GetInvoicesPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, string? status);
        Task<Invoice?> GetInvoiceByIdAsync(int companyId, int id);
        Task<int> CreateInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceStatusAsync(int companyId, int id, string status);
        Task<bool> DeleteInvoiceAsync(int companyId, int id);

        // Payments
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<decimal> GetTotalPaidByInvoiceAsync(int invoiceId);
        Task<int> CreatePaymentAsync(Payment payment);

        // Counter & Summary
        Task<int> GetInvoiceCountByCompanyAsync(int companyId);
        Task<(int TotalInvoices, decimal TotalInvoiced, decimal TotalPaid, int OverdueCount, decimal OverdueAmount)> GetReceivableSummaryAsync(int companyId);
    }
}
