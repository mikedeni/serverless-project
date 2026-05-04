namespace ConstructionSaaS.Api.DTOs
{
    public class ProjectReportDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal OutstandingReceivable { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMarginPercent { get; set; }
        public List<ExpenseBreakdownDto> ExpenseBreakdown { get; set; } = new();
    }

    public class ExpenseBreakdownDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
