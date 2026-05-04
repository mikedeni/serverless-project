using ConstructionSaaS.Api.DTOs;

namespace ConstructionSaaS.Api.Services
{
    public interface IReportService
    {
        Task<ProjectReportDto> GetProjectReportAsync(int companyId, int projectId);
    }
}
