namespace ConstructionSaaS.Api.DTOs
{
    // --- Worker ---
    public class CreateWorkerDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Position { get; set; }
        public decimal DailyWage { get; set; }
        public string? Phone { get; set; }
    }

    public class UpdateWorkerDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Position { get; set; }
        public decimal DailyWage { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "active";
    }

    // --- Attendance ---
    public class CreateAttendanceDto
    {
        public int WorkerId { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public string? CheckIn { get; set; }  // "08:00"
        public string? CheckOut { get; set; } // "17:00"
        public decimal OTHours { get; set; }
        public string? Note { get; set; }
    }

    public class BulkAttendanceDto
    {
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public List<AttendanceEntryDto> Entries { get; set; } = new();
    }

    public class AttendanceEntryDto
    {
        public int WorkerId { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public decimal OTHours { get; set; }
        public string? Note { get; set; }
    }

    // --- Summary ---
    public class DailyAttendanceSummaryDto
    {
        public DateTime Date { get; set; }
        public int WorkerCount { get; set; }
        public decimal TotalDailyWage { get; set; }
        public decimal TotalOTHours { get; set; }
    }
}
