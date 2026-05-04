namespace ConstructionSaaS.Api.DTOs
{
    // --- Create / Update ---
    public class CreateQuotationDto
    {
        public int ProjectId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhone { get; set; }
        public decimal MarkupPercent { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public string? Note { get; set; }
        public DateTime? ValidUntil { get; set; }
        public List<QuotationItemDto> Items { get; set; } = new();
    }

    public class UpdateQuotationDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhone { get; set; }
        public decimal MarkupPercent { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public string? Note { get; set; }
        public DateTime? ValidUntil { get; set; }
    }

    public class QuotationItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class UpdateQuotationStatusDto
    {
        public string Status { get; set; } = string.Empty; // draft, sent, approved, rejected
    }

    // --- Response ---
    public class QuotationDetailDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhone { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal MarkupPercent { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; }
        public string? Note { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConstructionSaaS.Api.Models.QuotationItem> Items { get; set; } = new();
        public QuotationSummaryDto Summary { get; set; } = new();
    }

    public class QuotationSummaryDto
    {
        public decimal SubTotal { get; set; }
        public decimal MarkupAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
