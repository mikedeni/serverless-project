using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryResponse> GetDashboardMetricsAsync(int companyId)
        {
            return await _dashboardRepository.GetDashboardSummaryAsync(companyId);
        }
    }
}
