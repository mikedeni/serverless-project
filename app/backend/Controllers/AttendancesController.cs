using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff,foreman")]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendancesController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public AttendancesController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetAttendances(int projectId, [FromQuery] DateTime? date)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var targetDate = date ?? DateTime.Today;
            var result = await _workerService.GetAttendancesByProjectAndDateAsync(companyId, projectId, targetDate);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RecordAttendance([FromBody] CreateAttendanceDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var attendance = await _workerService.RecordAttendanceAsync(companyId, dto);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkRecordAttendance([FromBody] BulkAttendanceDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var results = await _workerService.BulkRecordAttendanceAsync(companyId, dto);
                return Ok(new { recorded = results.Count(), message = $"Recorded {results.Count()} attendance entries." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
