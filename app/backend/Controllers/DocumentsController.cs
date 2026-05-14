using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentsController(IDocumentService service)
        {
            _service = service;
        }

        private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
        private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value ?? "0");

        [HttpGet("project/{projectId}")]
        [Authorize(Roles = "admin,staff,foreman,viewer")]
        public async Task<IActionResult> GetProjectDocuments(int projectId)
        {
            var companyId = GetCompanyId();
            return Ok(await _service.GetDocumentsByProjectAsync(companyId, projectId));
        }

        [HttpGet]
        [Authorize(Roles = "admin,staff,foreman,viewer")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var companyId = GetCompanyId();
            return Ok(await _service.GetAllDocumentsAsync(companyId));
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDto dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();
            var result = await _service.UploadDocumentAsync(companyId, userId, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var companyId = GetCompanyId();
            var success = await _service.DeleteDocumentAsync(companyId, id);
            if (!success) return NotFound("Document not found.");
            return Ok(new { message = "Document deleted." });
        }
    }
}
