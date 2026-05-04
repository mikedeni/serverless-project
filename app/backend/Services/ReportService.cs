using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class ReportService : IReportService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ReportService(
            IProjectRepository projectRepository,
            IExpenseRepository expenseRepository,
            IInvoiceRepository invoiceRepository)
        {
            _projectRepository = projectRepository;
            _expenseRepository = expenseRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ProjectReportDto> GetProjectReportAsync(int companyId, int projectId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(companyId, projectId);
            if (project == null)
                throw new Exception("Project not found or unauthorized.");

            // Get expenses
            var expenses = await _expenseRepository.GetExpensesByProjectIdAsync(companyId, projectId);
            var totalExpenses = expenses.Sum(e => e.Amount);

            // Get invoices for this project (all pages)
            var (invoices, _) = await _invoiceRepository.GetInvoicesPaginatedAsync(companyId, projectId, 0, 1000, null);
            var invoiceList = invoices.ToList();
            var totalInvoiced = invoiceList.Where(i => i.Status != "cancelled").Sum(i => i.TotalAmount);

            // Calculate paid amount from invoices
            decimal totalPaid = 0;
            foreach (var inv in invoiceList.Where(i => i.Status != "cancelled"))
            {
                totalPaid += await _invoiceRepository.GetTotalPaidByInvoiceAsync(inv.Id);
            }

            // Expense breakdown by category
            var breakdown = expenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseBreakdownDto
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    Percentage = totalExpenses > 0 ? Math.Round(g.Sum(e => e.Amount) / totalExpenses * 100, 1) : 0
                })
                .OrderByDescending(b => b.Amount)
                .ToList();

            var profit = totalPaid - totalExpenses;
            var profitMargin = totalPaid > 0 ? Math.Round(profit / totalPaid * 100, 1) : 0;

            return new ProjectReportDto
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                Status = project.Status,
                Budget = project.Budget,
                TotalExpenses = totalExpenses,
                TotalInvoiced = totalInvoiced,
                TotalPaid = totalPaid,
                OutstandingReceivable = totalInvoiced - totalPaid,
                Profit = profit,
                ProfitMarginPercent = profitMargin,
                ExpenseBreakdown = breakdown
            };
        }
    }
}
