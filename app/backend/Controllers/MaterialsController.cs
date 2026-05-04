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
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IFileStorageService _fileStorageService;

        public MaterialsController(IMaterialService materialService, IFileStorageService fileStorageService)
        {
            _materialService = materialService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterials([FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _materialService.GetMaterialsPaginatedAsync(companyId, query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterial(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var detail = await _materialService.GetMaterialDetailAsync(companyId, id);
            if (detail == null) return NotFound("Material not found.");

            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var material = await _materialService.CreateMaterialAsync(companyId, dto);
            return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, material);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] UpdateMaterialDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _materialService.UpdateMaterialAsync(companyId, id, dto);
            if (updated == null) return NotFound("Material not found or unauthorized.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _materialService.DeleteMaterialAsync(companyId, id);
            if (!success) return NotFound("Material not found.");

            return Ok(new { message = "Material deleted successfully." });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var materials = await _materialService.GetLowStockMaterialsAsync(companyId);
            return Ok(materials);
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> RecordTransaction([FromBody] CreateMaterialTransactionDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var transaction = await _materialService.RecordTransactionAsync(companyId, dto);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var detail = await _materialService.GetMaterialDetailAsync(companyId, id);
            if (detail == null) return NotFound("Material not found.");

            try 
            {
                var imageUrl = await _fileStorageService.SaveFileAsync(file, "materials");
                
                // Delete old image if exists
                if (!string.IsNullOrEmpty(detail.Material.ImageUrl))
                {
                    _fileStorageService.DeleteFile(detail.Material.ImageUrl);
                }

                await _materialService.UpdateImageUrlAsync(companyId, id, imageUrl);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
