using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IExpenseRepository
    {
        Task<int> CreateExpenseAsync(Expense expense);
        Task<Expense?> GetExpenseByIdAsync(int companyId, int id);
        Task<IEnumerable<Expense>> GetExpensesByProjectIdAsync(int companyId, int projectId);
        Task<decimal> GetTotalExpensesByProjectIdAsync(int companyId, int projectId);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int companyId, int id);
        Task<(IEnumerable<Expense> Items, int TotalCount)> GetExpensesPaginatedAsync(int companyId, int projectId, int offset, int pageSize, string? category);
    }
}
