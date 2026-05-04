namespace ConstructionSaaS.Api.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = "planning";
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Calculated fields
        public decimal TotalSpent { get; set; }
    }
}
