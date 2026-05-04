namespace ConstructionSaaS.Api.Models
{
    public class DailyReport
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Weather { get; set; } = "sunny";
        public int WorkerCount { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? Issues { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DailyReportPhoto
    {
        public int Id { get; set; }
        public int DailyReportId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
