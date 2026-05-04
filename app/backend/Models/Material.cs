namespace ConstructionSaaS.Api.Models
{
    public class Material
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal MinStock { get; set; }
        public decimal LastPrice { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Computed field
        public bool IsLowStock => CurrentStock <= MinStock;
    }
}
