using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class SubcontractorService : ISubcontractorService
    {
        private readonly ISubcontractorRepository _repo;
        private readonly IProjectRepository _projectRepo;

        public SubcontractorService(ISubcontractorRepository repo, IProjectRepository projectRepo)
        {
            _repo = repo;
            _projectRepo = projectRepo;
        }

        public async Task<PaginatedResponse<Subcontractor>> GetSubcontractorsPaginatedAsync(int companyId, PaginationQuery query)
        {
            var (items, totalCount) = await _repo.GetSubcontractorsPaginatedAsync(companyId, query.Offset, query.PageSize, query.Search, query.Status);
            return new PaginatedResponse<Subcontractor> { Items = items, TotalCount = totalCount, Page = query.Page, PageSize = query.PageSize };
        }

        public async Task<SubcontractorDetailDto?> GetSubcontractorDetailAsync(int companyId, int id)
        {
            var sub = await _repo.GetSubcontractorByIdAsync(companyId, id);
            if (sub == null) return null;

            var contracts = await _repo.GetContractsBySubAsync(companyId, id);
            return new SubcontractorDetailDto
            {
                Id = sub.Id, Name = sub.Name, Specialty = sub.Specialty, Phone = sub.Phone,
                Email = sub.Email, Status = sub.Status, CreatedAt = sub.CreatedAt,
                Contracts = contracts.ToList()
            };
        }

        public async Task<Subcontractor> CreateSubcontractorAsync(int companyId, CreateSubcontractorDto dto)
        {
            var sub = new Subcontractor
            {
                CompanyId = companyId, Name = dto.Name, Specialty = dto.Specialty,
                Phone = dto.Phone, Email = dto.Email, Status = "active"
            };
            sub.Id = await _repo.CreateSubcontractorAsync(sub);
            return sub;
        }

        public async Task<Subcontractor?> UpdateSubcontractorAsync(int companyId, int id, UpdateSubcontractorDto dto)
        {
            var existing = await _repo.GetSubcontractorByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Specialty = dto.Specialty;
            existing.Phone = dto.Phone;
            existing.Email = dto.Email;
            existing.Status = dto.Status;

            var success = await _repo.UpdateSubcontractorAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> DeleteSubcontractorAsync(int companyId, int id) => await _repo.DeleteSubcontractorAsync(companyId, id);

        public async Task<SubcontractorContract> CreateContractAsync(int companyId, CreateContractDto dto)
        {
            var project = await _projectRepo.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null) throw new Exception("Project not found.");

            var sub = await _repo.GetSubcontractorByIdAsync(companyId, dto.SubcontractorId);
            if (sub == null) throw new Exception("Subcontractor not found.");

            var contract = new SubcontractorContract
            {
                SubcontractorId = dto.SubcontractorId, ProjectId = dto.ProjectId, CompanyId = companyId,
                Scope = dto.Scope, ContractAmount = dto.ContractAmount, Status = "draft",
                StartDate = dto.StartDate, EndDate = dto.EndDate
            };
            contract.Id = await _repo.CreateContractAsync(contract);
            return contract;
        }

        public async Task<IEnumerable<SubcontractorContract>> GetContractsByProjectAsync(int companyId, int projectId) =>
            await _repo.GetContractsByProjectAsync(companyId, projectId);

        public async Task<bool> UpdateContractStatusAsync(int companyId, int id, string status)
        {
            var valid = new[] { "draft", "active", "completed", "cancelled" };
            if (!valid.Contains(status)) throw new Exception($"Invalid status: {status}");
            return await _repo.UpdateContractStatusAsync(companyId, id, status);
        }

        public async Task<SubcontractorPayment> RecordSubPaymentAsync(int companyId, int contractId, RecordSubPaymentDto dto)
        {
            var contract = await _repo.GetContractByIdAsync(companyId, contractId);
            if (contract == null) throw new Exception("Contract not found.");

            var remaining = contract.ContractAmount - contract.PaidAmount;
            if (dto.Amount <= 0) throw new Exception("Amount must be > 0.");
            if (dto.Amount > remaining) throw new Exception($"Amount ({dto.Amount:N2}) exceeds remaining ({remaining:N2}).");

            var payment = new SubcontractorPayment { ContractId = contractId, CompanyId = companyId, Amount = dto.Amount, PaymentDate = dto.PaymentDate, Note = dto.Note };
            payment.Id = await _repo.CreateSubPaymentAsync(payment);

            // Update paid amount
            await _repo.UpdateContractPaidAmountAsync(contractId, contract.PaidAmount + dto.Amount);

            // Auto-complete if fully paid
            if (contract.PaidAmount + dto.Amount >= contract.ContractAmount)
                await _repo.UpdateContractStatusAsync(companyId, contractId, "completed");

            return payment;
        }

        public async Task<PayableSummaryDto> GetPayableSummaryAsync(int companyId)
        {
            var (total, value, paid) = await _repo.GetPayableSummaryAsync(companyId);
            return new PayableSummaryDto { TotalContracts = total, TotalContractValue = value, TotalPaid = paid, TotalOutstanding = value - paid };
        }
    }
}
