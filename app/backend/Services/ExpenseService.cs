using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IProjectRepository _projectRepository;

        public ExpenseService(IExpenseRepository expenseRepository, IProjectRepository projectRepository)
        {
            _expenseRepository = expenseRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Expense> AddExpenseAsync(int companyId, Expense expenseData)
        {
            // Validate the project belongs to this company
            var project = await _projectRepository.GetProjectByIdAsync(companyId, expenseData.ProjectId);
            if (project == null)
            {
                throw new Exception("Project not found or you do not have permission.");
            }

            // Explicitly set CompanyId to enforce tenant isolation
            expenseData.CompanyId = companyId;
            
            var newId = await _expenseRepository.CreateExpenseAsync(expenseData);
            expenseData.Id = newId;

            return expenseData;
        }

        public async Task<IEnumerable<Expense>> GetExpensesByProjectAsync(int companyId, int projectId)
        {
            return await _expenseRepository.GetExpensesByProjectIdAsync(companyId, projectId);
        }

        public async Task<ProjectBudgetSummaryDto> GetBudgetSummaryAsync(int companyId, int projectId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(companyId, projectId);
            if (project == null)
            {
                throw new Exception("Project not found.");
            }

            var totalExpenses = await _expenseRepository.GetTotalExpensesByProjectIdAsync(companyId, projectId);

            return new ProjectBudgetSummaryDto
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                TotalBudget = project.Budget,
                TotalExpenses = totalExpenses
            };
        }

        public async Task<Expense?> UpdateExpenseAsync(int companyId, int id, Expense expenseData)
        {
            var existing = await _expenseRepository.GetExpenseByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.Amount = expenseData.Amount;
            existing.Category = expenseData.Category;
            existing.Date = expenseData.Date;
            existing.Note = expenseData.Note;

            var success = await _expenseRepository.UpdateExpenseAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> DeleteExpenseAsync(int companyId, int id)
        {
            return await _expenseRepository.DeleteExpenseAsync(companyId, id);
        }

        public async Task<PaginatedResponse<Expense>> GetExpensesPaginatedAsync(int companyId, int projectId, PaginationQuery query)
        {
            var (items, totalCount) = await _expenseRepository.GetExpensesPaginatedAsync(
                companyId, projectId, query.Offset, query.PageSize, query.Category);

            return new PaginatedResponse<Expense>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
