namespace ConstructionSaaS.Api.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
