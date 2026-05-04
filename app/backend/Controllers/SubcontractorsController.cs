using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class SubcontractorsController : ControllerBase
    {
        private readonly ISubcontractorService _service;

        public SubcontractorsController(ISubcontractorService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetSubcontractors([FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            return Ok(await _service.GetSubcontractorsPaginatedAsync(companyId, query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubcontractor(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var detail = await _service.GetSubcontractorDetailAsync(companyId, id);
            if (detail == null) return NotFound("Subcontractor not found.");
            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubcontractor([FromBody] CreateSubcontractorDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var sub = await _service.CreateSubcontractorAsync(companyId, dto);
            return CreatedAtAction(nameof(GetSubcontractor), new { id = sub.Id }, sub);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubcontractor(int id, [FromBody] UpdateSubcontractorDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var updated = await _service.UpdateSubcontractorAsync(companyId, id, dto);
            if (updated == null) return NotFound("Subcontractor not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcontractor(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var success = await _service.DeleteSubcontractorAsync(companyId, id);
            if (!success) return NotFound("Subcontractor not found.");
            return Ok(new { message = "Subcontractor deleted." });
        }

        // Contracts
        [HttpPost("contracts")]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            try
            {
                var contract = await _service.CreateContractAsync(companyId, dto);
                return Ok(contract);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("contracts/project/{projectId}")]
        public async Task<IActionResult> GetContractsByProject(int projectId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            return Ok(await _service.GetContractsByProjectAsync(companyId, projectId));
        }

        [HttpPut("contracts/{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] UpdateContractStatusDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            try
            {
                var success = await _service.UpdateContractStatusAsync(companyId, id, dto.Status);
                if (!success) return NotFound("Contract not found.");
                return Ok(new { message = $"Status updated to '{dto.Status}'." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("contracts/{id}/payments")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordSubPaymentDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            try
            {
                var payment = await _service.RecordSubPaymentAsync(companyId, id, dto);
                return Ok(payment);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("payable")]
        public async Task<IActionResult> GetPayableSummary()
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            return Ok(await _service.GetPayableSummaryAsync(companyId));
        }
    }
}
