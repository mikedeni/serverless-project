namespace ConstructionSaaS.Api.DTOs
{
    // --- Material ---
    public class CreateMaterialDto
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinStock { get; set; }
    }

    public class UpdateMaterialDto
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinStock { get; set; }
    }

    // --- Transaction ---
    public class CreateMaterialTransactionDto
    {
        public int MaterialId { get; set; }
        public int? ProjectId { get; set; }
        public string Type { get; set; } = string.Empty; // purchase_in, requisition_out, return, adjustment
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
    }

    // --- Response ---
    public class MaterialDetailDto
    {
        public ConstructionSaaS.Api.Models.Material Material { get; set; } = null!;
        public IEnumerable<ConstructionSaaS.Api.Models.MaterialTransaction> RecentTransactions { get; set; } = new List<ConstructionSaaS.Api.Models.MaterialTransaction>();
    }
}
