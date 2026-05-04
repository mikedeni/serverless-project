namespace ConstructionSaaS.Api.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "transfer";
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
