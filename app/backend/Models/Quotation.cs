namespace ConstructionSaaS.Api.Models
{
    public class Quotation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhone { get; set; }
        public string Status { get; set; } = "draft";
        public decimal MarkupPercent { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public string? Note { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class QuotationItem
    {
        public int Id { get; set; }
        public int QuotationId { get; set; }
        public int ItemOrder { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; } // Computed column in DB
        public DateTime CreatedAt { get; set; }
    }
}
