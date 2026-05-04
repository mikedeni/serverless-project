namespace ConstructionSaaS.Api.DTOs
{
    public class DashboardSummaryResponse
    {
        public int TotalProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalReceivables { get; set; }
        public decimal TotalPayables { get; set; }
        public decimal TotalRemainingBudget => TotalBudget - TotalExpenses;
        public decimal BudgetVsActualPercentage => TotalBudget == 0 ? 0 : Math.Round((TotalExpenses / TotalBudget) * 100, 2);
        
        public IEnumerable<ProjectProgressDto> ActiveProjectsProgress { get; set; } = new List<ProjectProgressDto>();
    }

    public class ProjectProgressDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalSpent { get; set; }
        
        public decimal ProgressPercentage => Budget == 0 ? 0 : Math.Round((TotalSpent / Budget) * 100, 2);
    }
}
