namespace ConstructionSaaS.Api.DTOs
{
    public class CreateDailyReportDto
    {
        public int ProjectId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Weather { get; set; } = "sunny";
        public int WorkerCount { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? Issues { get; set; }
        public List<DailyReportPhotoDto> Photos { get; set; } = new();
    }

    public class UpdateDailyReportDto
    {
        public string Weather { get; set; } = "sunny";
        public int WorkerCount { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? Issues { get; set; }
    }

    public class DailyReportPhotoDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }

    public class DailyReportDetailDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Weather { get; set; } = string.Empty;
        public int WorkerCount { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? Issues { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConstructionSaaS.Api.Models.DailyReportPhoto> Photos { get; set; } = new();
    }
}
