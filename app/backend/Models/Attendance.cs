namespace ConstructionSaaS.Api.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public decimal OTHours { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joined fields (from Workers table)
        public string? WorkerName { get; set; }
        public decimal DailyWage { get; set; }
    }
}
