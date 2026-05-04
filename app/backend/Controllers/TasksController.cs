using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Extensions;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff,foreman,viewer")]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var tasks = await _taskService.GetTasksByProjectAsync(companyId, projectId);
            return Ok(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff,foreman")]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem taskData)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var newTask = await _taskService.CreateTaskAsync(companyId, taskData);
                return Ok(newTask);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin,staff,foreman")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _taskService.UpdateTaskStatusAsync(companyId, id, dto);
            if (!success) return NotFound("Task not found or unauthorized.");

            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpPost("{id}/updates")]
        [Authorize(Roles = "admin,staff,foreman")]
        public async Task<IActionResult> AddSiteUpdate(int id, [FromBody] CreateTaskUpdateDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var updateResult = await _taskService.AddSiteUpdateAsync(companyId, id, dto);
                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}/updates")]
        public async Task<IActionResult> GetTaskUpdates(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var updates = await _taskService.GetTaskUpdatesAsync(companyId, id);
                return Ok(updates);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,staff,foreman")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _taskService.DeleteTaskAsync(companyId, id);
            if (!success) return NotFound("Task not found or unauthorized.");

            return Ok(new { Message = "Task deleted successfully." });
        }
    }
}
