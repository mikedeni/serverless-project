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
    public class WorkersController : ControllerBase
    {
        private readonly IWorkerService _workerService;
        private readonly IFileStorageService _fileStorageService;

        public WorkersController(IWorkerService workerService, IFileStorageService fileStorageService)
        {
            _workerService = workerService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkers([FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _workerService.GetWorkersPaginatedAsync(companyId, query);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllWorkers()
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var workers = await _workerService.GetAllWorkersAsync(companyId);
            return Ok(workers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorker(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var worker = await _workerService.GetWorkerByIdAsync(companyId, id);
            if (worker == null) return NotFound("Worker not found.");

            return Ok(worker);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorker([FromBody] CreateWorkerDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var worker = await _workerService.CreateWorkerAsync(companyId, dto);
            return CreatedAtAction(nameof(GetWorker), new { id = worker.Id }, worker);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker(int id, [FromBody] UpdateWorkerDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _workerService.UpdateWorkerAsync(companyId, id, dto);
            if (updated == null) return NotFound("Worker not found or unauthorized.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _workerService.DeleteWorkerAsync(companyId, id);
            if (!success) return NotFound("Worker not found.");

            return Ok(new { message = "Worker deleted successfully." });
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var worker = await _workerService.GetWorkerByIdAsync(companyId, id);
            if (worker == null) return NotFound("Worker not found.");

            try 
            {
                var imageUrl = await _fileStorageService.SaveFileAsync(file, "workers");
                
                // Delete old image if exists
                if (!string.IsNullOrEmpty(worker.ImageUrl))
                {
                    _fileStorageService.DeleteFile(worker.ImageUrl);
                }

                await _workerService.UpdateImageUrlAsync(companyId, id, imageUrl);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
