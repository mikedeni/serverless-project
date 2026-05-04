using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff,foreman,viewer")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IFileStorageService _fileStorageService;

        public ProjectsController(IProjectService projectService, IFileStorageService fileStorageService)
        {
            _projectService = projectService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _projectService.GetProjectsPaginatedAsync(companyId, query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var project = await _projectService.GetProjectByIdAsync(companyId, id);
            if (project == null) return NotFound("Project not found in your company workspace.");

            return Ok(project);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> CreateProject([FromBody] Project projectData)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var newProject = await _projectService.CreateProjectAsync(companyId, projectData);
            return CreatedAtAction(nameof(GetProject), new { id = newProject.Id }, newProject);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project projectData)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _projectService.UpdateProjectAsync(companyId, id, projectData);
            if (updated == null) return NotFound("Project not found or unauthorized.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _projectService.DeleteProjectAsync(companyId, id);
            if (!success) return NotFound("Project not found or unauthorized.");

            return Ok(new { Message = "Project deleted successfully." });
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            var companyId = Extensions.ClaimsPrincipalExtensions.GetCompanyId(User);
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var project = await _projectService.GetProjectByIdAsync(companyId, id);
            if (project == null) return NotFound("Project not found.");

            try 
            {
                var imageUrl = await _fileStorageService.SaveFileAsync(file, "projects");
                
                // Delete old image if exists
                if (!string.IsNullOrEmpty(project.ImageUrl))
                {
                    _fileStorageService.DeleteFile(project.ImageUrl);
                }

                await _projectService.UpdateImageUrlAsync(companyId, id, imageUrl);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
