namespace ConstructionSaaS.Api.DTOs
{
    public class CreateSubcontractorDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateSubcontractorDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = "active";
    }

    public class CreateContractDto
    {
        public int SubcontractorId { get; set; }
        public int ProjectId { get; set; }
        public string Scope { get; set; } = string.Empty;
        public decimal ContractAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateContractStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class RecordSubPaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Note { get; set; }
    }

    public class SubcontractorDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ConstructionSaaS.Api.Models.SubcontractorContract> Contracts { get; set; } = new();
    }

    public class PayableSummaryDto
    {
        public int TotalContracts { get; set; }
        public decimal TotalContractValue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
