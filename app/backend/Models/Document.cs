namespace ConstructionSaaS.Api.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int CompanyId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public string Category { get; set; } = "other";
        public int? UploadedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
