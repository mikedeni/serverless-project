using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IWorkerService
    {
        // Workers
        Task<PaginatedResponse<Worker>> GetWorkersPaginatedAsync(int companyId, PaginationQuery query);
        Task<IEnumerable<Worker>> GetAllWorkersAsync(int companyId);
        Task<Worker?> GetWorkerByIdAsync(int companyId, int id);
        Task<Worker> CreateWorkerAsync(int companyId, CreateWorkerDto dto);
        Task<Worker?> UpdateWorkerAsync(int companyId, int id, UpdateWorkerDto dto);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
        Task<bool> DeleteWorkerAsync(int companyId, int id);

        // Attendances
        Task<IEnumerable<Attendance>> GetAttendancesByProjectAndDateAsync(int companyId, int projectId, DateTime date);
        Task<Attendance> RecordAttendanceAsync(int companyId, CreateAttendanceDto dto);
        Task<IEnumerable<Attendance>> BulkRecordAttendanceAsync(int companyId, BulkAttendanceDto dto);
    }
}
