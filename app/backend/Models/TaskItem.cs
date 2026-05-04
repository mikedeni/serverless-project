namespace ConstructionSaaS.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public int? AssignedUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; }
    }
}
