namespace ConstructionSaaS.Api.DTOs
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Type { get; set; } = "general";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RelatedUrl { get; set; }
    }

    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RelatedUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
