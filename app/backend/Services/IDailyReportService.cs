using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IDailyReportService
    {
        Task<PaginatedResponse<DailyReport>> GetReportsPaginatedAsync(int companyId, int projectId, PaginationQuery query, DateTime? startDate, DateTime? endDate);
        Task<DailyReportDetailDto?> GetReportDetailAsync(int companyId, int id);
        Task<DailyReport> CreateReportAsync(int companyId, int userId, CreateDailyReportDto dto);
        Task<DailyReport?> UpdateReportAsync(int companyId, int id, UpdateDailyReportDto dto);
        Task<bool> DeleteReportAsync(int companyId, int id);
    }
}
