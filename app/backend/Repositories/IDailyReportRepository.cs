using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IDailyReportRepository
    {
        Task<(IEnumerable<DailyReport> Items, int TotalCount)> GetReportsPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, DateTime? startDate, DateTime? endDate);
        Task<DailyReport?> GetReportByIdAsync(int companyId, int id);
        Task<DailyReport?> GetReportByDateAsync(int companyId, int projectId, DateTime reportDate);
        Task<int> CreateReportAsync(DailyReport report);
        Task<bool> UpdateReportAsync(DailyReport report);
        Task<bool> DeleteReportAsync(int companyId, int id);

        // Photos
        Task<IEnumerable<DailyReportPhoto>> GetPhotosByReportAsync(int reportId);
        Task<int> AddPhotoAsync(DailyReportPhoto photo);
    }
}
