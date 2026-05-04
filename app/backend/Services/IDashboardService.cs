using ConstructionSaaS.Api.DTOs;

namespace ConstructionSaaS.Api.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetDashboardMetricsAsync(int companyId);
    }
}
