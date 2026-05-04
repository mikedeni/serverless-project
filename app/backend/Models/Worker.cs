namespace ConstructionSaaS.Api.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Position { get; set; }
        public decimal DailyWage { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "active";
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
