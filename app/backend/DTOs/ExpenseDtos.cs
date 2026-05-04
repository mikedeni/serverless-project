namespace ConstructionSaaS.Api.DTOs
{
    public class ProjectBudgetSummaryDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal TotalBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal RemainingBudget => TotalBudget - TotalExpenses;
        
        public decimal VariancePercentage => TotalBudget == 0 ? 0 : Math.Round((TotalExpenses / TotalBudget) * 100, 2);
    }
}
