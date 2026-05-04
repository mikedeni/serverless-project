namespace ConstructionSaaS.Api.DTOs
{
    // --- Create / Update ---
    public class CreateInvoiceDto
    {
        public int ProjectId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public DateTime? DueDate { get; set; }
        public string? MilestoneLabel { get; set; }
    }

    public class UpdateInvoiceDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public DateTime? DueDate { get; set; }
        public string? MilestoneLabel { get; set; }
    }

    public class UpdateInvoiceStatusDto
    {
        public string Status { get; set; } = string.Empty; // draft, sent, paid, overdue, cancelled
    }

    public class RecordPaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "transfer"; // cash, transfer, cheque, other
        public string? Note { get; set; }
    }

    // --- Response ---
    public class InvoiceDetailDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? MilestoneLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public List<ConstructionSaaS.Api.Models.Payment> Payments { get; set; } = new();
    }

    public class ReceivableSummaryDto
    {
        public int TotalInvoices { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverdueCount { get; set; }
        public decimal OverdueAmount { get; set; }
    }
}
