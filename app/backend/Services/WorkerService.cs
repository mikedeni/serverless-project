using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IProjectRepository _projectRepository;

        public WorkerService(IWorkerRepository workerRepository, IProjectRepository projectRepository)
        {
            _workerRepository = workerRepository;
            _projectRepository = projectRepository;
        }

        // --- Workers ---

        public async Task<PaginatedResponse<Worker>> GetWorkersPaginatedAsync(int companyId, PaginationQuery query)
        {
            var (items, totalCount) = await _workerRepository.GetWorkersPaginatedAsync(
                companyId, query.Offset, query.PageSize, query.Search, query.Status);

            return new PaginatedResponse<Worker>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<IEnumerable<Worker>> GetAllWorkersAsync(int companyId)
        {
            return await _workerRepository.GetWorkersByCompanyAsync(companyId);
        }

        public async Task<Worker?> GetWorkerByIdAsync(int companyId, int id)
        {
            return await _workerRepository.GetWorkerByIdAsync(companyId, id);
        }

        public async Task<Worker> CreateWorkerAsync(int companyId, CreateWorkerDto dto)
        {
            var worker = new Worker
            {
                CompanyId = companyId,
                Name = dto.Name,
                Position = dto.Position,
                DailyWage = dto.DailyWage,
                Phone = dto.Phone,
                Status = "active"
            };

            var id = await _workerRepository.CreateWorkerAsync(worker);
            worker.Id = id;
            return worker;
        }

        public async Task<Worker?> UpdateWorkerAsync(int companyId, int id, UpdateWorkerDto dto)
        {
            var existing = await _workerRepository.GetWorkerByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Position = dto.Position;
            existing.DailyWage = dto.DailyWage;
            existing.Phone = dto.Phone;
            existing.Status = dto.Status;

            var success = await _workerRepository.UpdateWorkerAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            return await _workerRepository.UpdateImageUrlAsync(companyId, id, imageUrl);
        }

        public async Task<bool> DeleteWorkerAsync(int companyId, int id)
        {
            return await _workerRepository.DeleteWorkerAsync(companyId, id);
        }

        // --- Attendances ---

        public async Task<IEnumerable<Attendance>> GetAttendancesByProjectAndDateAsync(int companyId, int projectId, DateTime date)
        {
            return await _workerRepository.GetAttendancesByProjectAndDateAsync(companyId, projectId, date);
        }

        public async Task<Attendance> RecordAttendanceAsync(int companyId, CreateAttendanceDto dto)
        {
            // Validate project
            var project = await _projectRepository.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found or unauthorized.");

            // Validate worker
            var worker = await _workerRepository.GetWorkerByIdAsync(companyId, dto.WorkerId);
            if (worker == null)
                throw new Exception("Worker not found or unauthorized.");

            // Check for duplicate
            var exists = await _workerRepository.AttendanceExistsAsync(dto.WorkerId, dto.ProjectId, dto.Date);
            if (exists)
                throw new Exception($"Attendance already recorded for worker '{worker.Name}' on {dto.Date:yyyy-MM-dd} in this project.");

            var attendance = new Attendance
            {
                WorkerId = dto.WorkerId,
                ProjectId = dto.ProjectId,
                CompanyId = companyId,
                Date = dto.Date.Date,
                CheckIn = ParseTime(dto.CheckIn),
                CheckOut = ParseTime(dto.CheckOut),
                OTHours = dto.OTHours,
                Note = dto.Note
            };

            var id = await _workerRepository.CreateAttendanceAsync(attendance);
            attendance.Id = id;
            attendance.WorkerName = worker.Name;
            attendance.DailyWage = worker.DailyWage;

            return attendance;
        }

        public async Task<IEnumerable<Attendance>> BulkRecordAttendanceAsync(int companyId, BulkAttendanceDto dto)
        {
            // Validate project
            var project = await _projectRepository.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found or unauthorized.");

            var results = new List<Attendance>();

            foreach (var entry in dto.Entries)
            {
                // Skip if already recorded
                var exists = await _workerRepository.AttendanceExistsAsync(entry.WorkerId, dto.ProjectId, dto.Date);
                if (exists) continue;

                var worker = await _workerRepository.GetWorkerByIdAsync(companyId, entry.WorkerId);
                if (worker == null) continue;

                var attendance = new Attendance
                {
                    WorkerId = entry.WorkerId,
                    ProjectId = dto.ProjectId,
                    CompanyId = companyId,
                    Date = dto.Date.Date,
                    CheckIn = ParseTime(entry.CheckIn),
                    CheckOut = ParseTime(entry.CheckOut),
                    OTHours = entry.OTHours,
                    Note = entry.Note
                };

                var id = await _workerRepository.CreateAttendanceAsync(attendance);
                attendance.Id = id;
                attendance.WorkerName = worker.Name;
                attendance.DailyWage = worker.DailyWage;
                results.Add(attendance);
            }

            return results;
        }

        private TimeSpan? ParseTime(string? timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return null;
            return TimeSpan.TryParse(timeStr, out var time) ? time : null;
        }
    }
}
