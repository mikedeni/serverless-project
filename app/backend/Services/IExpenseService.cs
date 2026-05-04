using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IExpenseService
    {
        Task<Expense> AddExpenseAsync(int companyId, Expense expenseData);
        Task<IEnumerable<Expense>> GetExpensesByProjectAsync(int companyId, int projectId);
        Task<ProjectBudgetSummaryDto> GetBudgetSummaryAsync(int companyId, int projectId);
        Task<Expense?> UpdateExpenseAsync(int companyId, int id, Expense expenseData);
        Task<bool> DeleteExpenseAsync(int companyId, int id);
        Task<PaginatedResponse<Expense>> GetExpensesPaginatedAsync(int companyId, int projectId, PaginationQuery query);
    }
}
