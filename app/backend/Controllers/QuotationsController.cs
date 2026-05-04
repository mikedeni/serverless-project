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
    public class QuotationsController : ControllerBase
    {
        private readonly IQuotationService _quotationService;

        public QuotationsController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetQuotations(int projectId, [FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _quotationService.GetQuotationsPaginatedAsync(companyId, projectId, query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuotation(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var detail = await _quotationService.GetQuotationDetailAsync(companyId, id);
            if (detail == null) return NotFound("Quotation not found.");

            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuotation([FromBody] CreateQuotationDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var quotation = await _quotationService.CreateQuotationAsync(companyId, dto);
                return CreatedAtAction(nameof(GetQuotation), new { id = quotation.Id }, quotation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotation(int id, [FromBody] UpdateQuotationDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _quotationService.UpdateQuotationAsync(companyId, id, dto);
            if (updated == null) return NotFound("Quotation not found or unauthorized.");

            return Ok(updated);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateQuotationStatusDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var success = await _quotationService.UpdateQuotationStatusAsync(companyId, id, dto.Status);
                if (!success) return NotFound("Quotation not found.");

                return Ok(new { message = $"Status updated to '{dto.Status}'." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotation(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _quotationService.DeleteQuotationAsync(companyId, id);
            if (!success) return NotFound("Quotation not found.");

            return Ok(new { message = "Quotation deleted successfully." });
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItem(int id, [FromBody] QuotationItemDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var item = await _quotationService.AddQuotationItemAsync(companyId, id, dto);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> DeleteItem(int id, int itemId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var success = await _quotationService.DeleteQuotationItemAsync(companyId, id, itemId);
                if (!success) return NotFound("Item not found.");

                return Ok(new { message = "Item removed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
