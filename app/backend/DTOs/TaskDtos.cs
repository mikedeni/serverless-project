using System.ComponentModel.DataAnnotations;

namespace ConstructionSaaS.Api.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class CreateTaskUpdateDto
    {
        [Required]
        public string Note { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
