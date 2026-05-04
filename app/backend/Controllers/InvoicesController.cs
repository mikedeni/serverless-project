using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetInvoices(int projectId, [FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _invoiceService.GetInvoicesPaginatedAsync(companyId, projectId, query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var detail = await _invoiceService.GetInvoiceDetailAsync(companyId, id);
            if (detail == null) return NotFound("Invoice not found.");

            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(companyId, dto);
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _invoiceService.UpdateInvoiceAsync(companyId, id, dto);
            if (updated == null) return NotFound("Invoice not found or unauthorized.");

            return Ok(updated);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInvoiceStatusDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var success = await _invoiceService.UpdateInvoiceStatusAsync(companyId, id, dto.Status);
                if (!success) return NotFound("Invoice not found.");

                return Ok(new { message = $"Status updated to '{dto.Status}'." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _invoiceService.DeleteInvoiceAsync(companyId, id);
            if (!success) return NotFound("Invoice not found.");

            return Ok(new { message = "Invoice deleted successfully." });
        }

        [HttpPost("{id}/payments")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordPaymentDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var payment = await _invoiceService.RecordPaymentAsync(companyId, id, dto);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("receivable")]
        public async Task<IActionResult> GetReceivableSummary()
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var summary = await _invoiceService.GetReceivableSummaryAsync(companyId);
            return Ok(summary);
        }
    }
}
