namespace ConstructionSaaS.Api.Models
{
    public class TaskUpdate
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Note { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
