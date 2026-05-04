namespace ConstructionSaaS.Api.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; } = 7.00m;
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "draft";
        public string? MilestoneLabel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
