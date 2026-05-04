using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IWorkerRepository
    {
        // Workers
        Task<IEnumerable<Worker>> GetWorkersByCompanyAsync(int companyId);
        Task<(IEnumerable<Worker> Items, int TotalCount)> GetWorkersPaginatedAsync(
            int companyId, int offset, int pageSize, string? search, string? status);
        Task<Worker?> GetWorkerByIdAsync(int companyId, int id);
        Task<int> CreateWorkerAsync(Worker worker);
        Task<bool> UpdateWorkerAsync(Worker worker);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
        Task<bool> DeleteWorkerAsync(int companyId, int id);

        // Attendances
        Task<IEnumerable<Attendance>> GetAttendancesByProjectAndDateAsync(int companyId, int projectId, DateTime date);
        Task<int> CreateAttendanceAsync(Attendance attendance);
        Task<bool> AttendanceExistsAsync(int workerId, int projectId, DateTime date);
        Task<IEnumerable<Attendance>> GetAttendancesByProjectAsync(int companyId, int projectId, DateTime? dateFrom, DateTime? dateTo);
    }
}
