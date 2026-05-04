using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IInvoiceService _invoiceService;
        private readonly IQuotationService _quotationService;
        private readonly PdfService _pdfService;

        public ReportsController(IReportService reportService, IInvoiceService invoiceService, IQuotationService quotationService, PdfService pdfService)
        {
            _reportService = reportService;
            _invoiceService = invoiceService;
            _quotationService = quotationService;
            _pdfService = pdfService;
        }

        /// <summary>
        /// Get project financial summary (JSON)
        /// </summary>
        [HttpGet("project/{projectId}/summary")]
        public async Task<IActionResult> GetProjectSummary(int projectId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var report = await _reportService.GetProjectReportAsync(companyId, projectId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Export project financial summary as PDF
        /// </summary>
        [HttpGet("project/{projectId}/summary/pdf")]
        public async Task<IActionResult> ExportProjectSummaryPdf(int projectId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var report = await _reportService.GetProjectReportAsync(companyId, projectId);
                var pdfBytes = _pdfService.GenerateProjectReport(report);
                return File(pdfBytes, "application/pdf", $"Report_{report.ProjectName}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Export invoice as PDF
        /// </summary>
        [HttpGet("invoice/{id}/pdf")]
        public async Task<IActionResult> ExportInvoicePdf(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var invoice = await _invoiceService.GetInvoiceDetailAsync(companyId, id);
            if (invoice == null) return NotFound("Invoice not found.");

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
        }

        /// <summary>
        /// Export quotation as PDF
        /// </summary>
        [HttpGet("quotation/{id}/pdf")]
        public async Task<IActionResult> ExportQuotationPdf(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var quotation = await _quotationService.GetQuotationDetailAsync(companyId, id);
            if (quotation == null) return NotFound("Quotation not found.");

            var pdfBytes = _pdfService.GenerateQuotationPdf(quotation);
            return File(pdfBytes, "application/pdf", $"Quotation_{quotation.QuotationNumber}.pdf");
        }
    }
}
