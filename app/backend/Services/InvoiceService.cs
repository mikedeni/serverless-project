using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProjectRepository _projectRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IProjectRepository projectRepository)
        {
            _invoiceRepository = invoiceRepository;
            _projectRepository = projectRepository;
        }

        public async Task<PaginatedResponse<Invoice>> GetInvoicesPaginatedAsync(int companyId, int projectId, PaginationQuery query)
        {
            var (items, totalCount) = await _invoiceRepository.GetInvoicesPaginatedAsync(
                companyId, projectId, query.Offset, query.PageSize, query.Status);

            return new PaginatedResponse<Invoice>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<InvoiceDetailDto?> GetInvoiceDetailAsync(int companyId, int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(companyId, id);
            if (invoice == null) return null;

            var payments = await _invoiceRepository.GetPaymentsByInvoiceAsync(id);
            var paidAmount = await _invoiceRepository.GetTotalPaidByInvoiceAsync(id);

            return new InvoiceDetailDto
            {
                Id = invoice.Id,
                ProjectId = invoice.ProjectId,
                InvoiceNumber = invoice.InvoiceNumber,
                ClientName = invoice.ClientName,
                Description = invoice.Description,
                Amount = invoice.Amount,
                TaxPercent = invoice.TaxPercent,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                MilestoneLabel = invoice.MilestoneLabel,
                CreatedAt = invoice.CreatedAt,
                PaidAmount = paidAmount,
                RemainingBalance = invoice.TotalAmount - paidAmount,
                Payments = payments.ToList()
            };
        }

        public async Task<Invoice> CreateInvoiceAsync(int companyId, CreateInvoiceDto dto)
        {
            // Validate project ownership
            var project = await _projectRepository.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found or you do not have permission.");

            // Auto-generate invoice number
            var count = await _invoiceRepository.GetInvoiceCountByCompanyAsync(companyId);
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMM}-{(count + 1):D4}";

            // Calculate tax
            var taxAmount = Math.Round(dto.Amount * (dto.TaxPercent / 100m), 2);
            var totalAmount = Math.Round(dto.Amount + taxAmount, 2);

            var invoice = new Invoice
            {
                ProjectId = dto.ProjectId,
                CompanyId = companyId,
                InvoiceNumber = invoiceNumber,
                ClientName = dto.ClientName,
                Description = dto.Description,
                Amount = dto.Amount,
                TaxPercent = dto.TaxPercent,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                DueDate = dto.DueDate,
                Status = "draft",
                MilestoneLabel = dto.MilestoneLabel
            };

            var invoiceId = await _invoiceRepository.CreateInvoiceAsync(invoice);
            invoice.Id = invoiceId;

            return invoice;
        }

        public async Task<Invoice?> UpdateInvoiceAsync(int companyId, int id, UpdateInvoiceDto dto)
        {
            var existing = await _invoiceRepository.GetInvoiceByIdAsync(companyId, id);
            if (existing == null) return null;

            // Recalculate tax
            var taxAmount = Math.Round(dto.Amount * (dto.TaxPercent / 100m), 2);
            var totalAmount = Math.Round(dto.Amount + taxAmount, 2);

            existing.ClientName = dto.ClientName;
            existing.Description = dto.Description;
            existing.Amount = dto.Amount;
            existing.TaxPercent = dto.TaxPercent;
            existing.TaxAmount = taxAmount;
            existing.TotalAmount = totalAmount;
            existing.DueDate = dto.DueDate;
            existing.MilestoneLabel = dto.MilestoneLabel;

            var success = await _invoiceRepository.UpdateInvoiceAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int companyId, int id, string status)
        {
            var validStatuses = new[] { "draft", "sent", "paid", "overdue", "cancelled" };
            if (!validStatuses.Contains(status))
                throw new Exception($"Invalid status: {status}. Valid values: {string.Join(", ", validStatuses)}");

            return await _invoiceRepository.UpdateInvoiceStatusAsync(companyId, id, status);
        }

        public async Task<bool> DeleteInvoiceAsync(int companyId, int id)
        {
            return await _invoiceRepository.DeleteInvoiceAsync(companyId, id);
        }

        public async Task<Payment> RecordPaymentAsync(int companyId, int invoiceId, RecordPaymentDto dto)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(companyId, invoiceId);
            if (invoice == null)
                throw new Exception("Invoice not found or unauthorized.");

            // Validate payment amount
            var totalPaid = await _invoiceRepository.GetTotalPaidByInvoiceAsync(invoiceId);
            var remaining = invoice.TotalAmount - totalPaid;

            if (dto.Amount <= 0)
                throw new Exception("Payment amount must be greater than 0.");

            if (dto.Amount > remaining)
                throw new Exception($"Payment amount ({dto.Amount:N2}) exceeds remaining balance ({remaining:N2}).");

            // Validate method
            var validMethods = new[] { "cash", "transfer", "cheque", "other" };
            if (!validMethods.Contains(dto.Method))
                throw new Exception($"Invalid payment method: {dto.Method}. Valid values: {string.Join(", ", validMethods)}");

            var payment = new Payment
            {
                InvoiceId = invoiceId,
                CompanyId = companyId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Method = dto.Method,
                Note = dto.Note
            };

            var paymentId = await _invoiceRepository.CreatePaymentAsync(payment);
            payment.Id = paymentId;

            // Auto-mark as paid if fully paid
            var newTotalPaid = totalPaid + dto.Amount;
            if (newTotalPaid >= invoice.TotalAmount)
            {
                await _invoiceRepository.UpdateInvoiceStatusAsync(companyId, invoiceId, "paid");
            }

            return payment;
        }

        public async Task<ReceivableSummaryDto> GetReceivableSummaryAsync(int companyId)
        {
            var (totalInvoices, totalInvoiced, totalPaid, overdueCount, overdueAmount) =
                await _invoiceRepository.GetReceivableSummaryAsync(companyId);

            return new ReceivableSummaryDto
            {
                TotalInvoices = totalInvoices,
                TotalInvoiced = totalInvoiced,
                TotalPaid = totalPaid,
                TotalOutstanding = totalInvoiced - totalPaid,
                OverdueCount = overdueCount,
                OverdueAmount = overdueAmount
            };
        }
    }
}
