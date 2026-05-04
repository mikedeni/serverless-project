using Prometheus;

namespace ConstructionSaaS.Api.Monitoring
{
    public static class BusinessMetrics
    {
        public static readonly Gauge ProjectCount = Metrics.CreateGauge(
            "mybrick_projects_total", 
            "Total number of projects across all companies");

        public static readonly Gauge TotalExpenseAmount = Metrics.CreateGauge(
            "mybrick_expenses_total_baht", 
            "Total spending amount across all companies in THB");
            
        public static readonly Counter ErrorCounter = Metrics.CreateCounter(
            "mybrick_business_errors_total", 
            "Total number of business logic errors encountered",
            new CounterConfiguration
            {
                LabelNames = new[] { "error_type" }
            });
    }
}
