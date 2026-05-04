using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ConstructionSaaS.Api.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        public int? ProjectId { get; set; }
        public string Category { get; set; } = "other";
    }

    public class CreateDocumentDto
    {
        public int? ProjectId { get; set; }
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string FileUrl { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public string Category { get; set; } = "other";
    }

    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public string Category { get; set; } = string.Empty;
        public int? UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
