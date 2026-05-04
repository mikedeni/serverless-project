using ConstructionSaaS.Api.DTOs;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int companyId);
    }
}
