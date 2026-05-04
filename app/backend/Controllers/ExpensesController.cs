using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Extensions;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin,staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] Expense expenseData)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var newExpense = await _expenseService.AddExpenseAsync(companyId, expenseData);
                return CreatedAtAction(nameof(GetExpensesByProject), new { projectId = newExpense.ProjectId }, newExpense);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetExpensesByProject(int projectId, [FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var result = await _expenseService.GetExpensesPaginatedAsync(companyId, projectId, query);
            return Ok(result);
        }

        [HttpGet("project/{projectId}/budget-summary")]
        public async Task<IActionResult> GetBudgetSummary(int projectId)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var summary = await _expenseService.GetBudgetSummaryAsync(companyId, projectId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense expenseData)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var updated = await _expenseService.UpdateExpenseAsync(companyId, id, expenseData);
            if (updated == null) return NotFound("Expense not found or unauthorized.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var success = await _expenseService.DeleteExpenseAsync(companyId, id);
            if (!success) return NotFound("Expense not found or unauthorized.");

            return Ok(new { Message = "Expense deleted successfully." });
        }
    }
}
