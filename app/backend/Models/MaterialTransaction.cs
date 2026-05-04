namespace ConstructionSaaS.Api.Models
{
    public class MaterialTransaction
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int? ProjectId { get; set; }
        public int CompanyId { get; set; }
        public string Type { get; set; } = string.Empty; // purchase_in, requisition_out, return, adjustment
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joined fields
        public string? MaterialName { get; set; }
        public string? ProjectName { get; set; }
    }
}
