namespace ConstructionSaaS.Api.Models
{
    public class Subcontractor
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
    }

    public class SubcontractorContract
    {
        public int Id { get; set; }
        public int SubcontractorId { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public string Scope { get; set; } = string.Empty;
        public decimal ContractAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = "draft";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joined fields
        public string? SubcontractorName { get; set; }
        public string? ProjectName { get; set; }
    }

    public class SubcontractorPayment
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
