using ConstructionSaaS.Api.Data;
using ConstructionSaaS.Api.DTOs;
using Dapper;

namespace ConstructionSaaS.Api.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DapperContext _context;

        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int companyId)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                -- 1. Get Top Level Aggregates (Projects Context)
                SELECT COUNT(Id) AS TotalProjects, COALESCE(SUM(Budget), 0) AS TotalBudget 
                FROM Projects 
                WHERE CompanyId = @CompanyId;

                -- 2. Get Top Level Aggregates (Global Expenses Context - Optimized)
                SELECT SUM(Total) AS TotalExpenses FROM (
                    SELECT COALESCE(SUM(Amount), 0) AS Total FROM Expenses WHERE CompanyId = @CompanyId
                    UNION ALL
                    SELECT COALESCE(SUM(Qty * UnitPrice), 0) AS Total FROM MaterialTransactions WHERE CompanyId = @CompanyId AND Type = 'purchase_in'
                    UNION ALL
                    SELECT COALESCE(SUM(Amount), 0) AS Total FROM SubcontractorPayments WHERE CompanyId = @CompanyId
                    UNION ALL
                    SELECT COALESCE(SUM(w.DailyWage), 0) AS Total 
                    FROM Attendances a 
                    JOIN Workers w ON a.WorkerId = w.Id 
                    WHERE a.CompanyId = @CompanyId
                ) AS global_costs;

                -- 3. Get Receivables
                SELECT COALESCE(SUM(
                    TotalAmount - (SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE InvoiceId = Invoices.Id)
                ), 0) AS TotalReceivables
                FROM Invoices
                WHERE CompanyId = @CompanyId AND Status NOT IN ('draft', 'cancelled', 'paid');

                -- 4. Get Payables
                SELECT COALESCE(SUM(ContractAmount - PaidAmount), 0) AS TotalPayables
                FROM SubcontractorContracts
                WHERE CompanyId = @CompanyId AND Status = 'active';

                -- 5. Get Project List details (Optimized Aggregation)
                SELECT 
                    p.Id, 
                    p.ProjectName, 
                    p.Status, 
                    p.Budget, 
                    COALESCE(costs.TotalSpent, 0) AS TotalSpent
                FROM Projects p
                LEFT JOIN (
                    SELECT ProjectId, SUM(Amount) AS TotalSpent FROM (
                        SELECT ProjectId, Amount FROM Expenses WHERE CompanyId = @CompanyId
                        UNION ALL
                        SELECT ProjectId, (Qty * UnitPrice) FROM MaterialTransactions WHERE CompanyId = @CompanyId AND Type = 'purchase_in'
                        UNION ALL
                        SELECT sc.ProjectId, sp.Amount FROM SubcontractorPayments sp JOIN SubcontractorContracts sc ON sp.ContractId = sc.Id WHERE sp.CompanyId = @CompanyId
                        UNION ALL
                        SELECT a.ProjectId, w.DailyWage FROM Attendances a JOIN Workers w ON a.WorkerId = w.Id WHERE a.CompanyId = @CompanyId
                    ) combined_costs GROUP BY ProjectId
                ) costs ON p.Id = costs.ProjectId
                WHERE p.CompanyId = @CompanyId
                ORDER BY p.CreatedAt DESC;";

            using var multi = await connection.QueryMultipleAsync(sql, new { CompanyId = companyId });

            var projectsSummary = await multi.ReadSingleAsync<dynamic>();
            var expensesSummary = await multi.ReadSingleAsync<dynamic>();
            var receivablesSummary = await multi.ReadSingleAsync<dynamic>();
            var payablesSummary = await multi.ReadSingleAsync<dynamic>();
            var projectList = await multi.ReadAsync<ProjectProgressDto>();

            var response = new DashboardSummaryResponse
            {
                TotalProjects = Convert.ToInt32(projectsSummary.TotalProjects),
                TotalBudget = Convert.ToDecimal(projectsSummary.TotalBudget),
                TotalExpenses = Convert.ToDecimal(expensesSummary.TotalExpenses),
                TotalReceivables = Convert.ToDecimal(receivablesSummary.TotalReceivables),
                TotalPayables = Convert.ToDecimal(payablesSummary.TotalPayables),
                ActiveProjectsProgress = projectList
            };

            return response;
        }
    }
}
