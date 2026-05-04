using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff,foreman")]
    [ApiController]
    [Route("api/daily-reports")]
    public class DailyReportsController : ControllerBase
    {
        private readonly IDailyReportService _service;

        public DailyReportsController(IDailyReportService service) { _service = service; }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetReports(int projectId, [FromQuery] PaginationQuery query, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            return Ok(await _service.GetReportsPaginatedAsync(companyId, projectId, query, startDate, endDate));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var detail = await _service.GetReportDetailAsync(companyId, id);
            if (detail == null) return NotFound("Report not found.");
            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateDailyReportDto dto)
        {
            var companyId = User.GetCompanyId();
            var userId = User.GetUserId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            try
            {
                var report = await _service.CreateReportAsync(companyId, userId, dto);
                return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateDailyReportDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var updated = await _service.UpdateReportAsync(companyId, id, dto);
            if (updated == null) return NotFound("Report not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var success = await _service.DeleteReportAsync(companyId, id);
            if (!success) return NotFound("Report not found.");
            return Ok(new { message = "Report deleted." });
        }
    }
}
