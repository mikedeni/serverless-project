using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class DailyReportService : IDailyReportService
    {
        private readonly IDailyReportRepository _repo;
        private readonly IProjectRepository _projectRepo;

        public DailyReportService(IDailyReportRepository repo, IProjectRepository projectRepo)
        {
            _repo = repo;
            _projectRepo = projectRepo;
        }

        public async Task<PaginatedResponse<DailyReport>> GetReportsPaginatedAsync(int companyId, int projectId, PaginationQuery query, DateTime? startDate, DateTime? endDate)
        {
            var (items, totalCount) = await _repo.GetReportsPaginatedAsync(companyId, projectId, query.Offset, query.PageSize, startDate, endDate);
            return new PaginatedResponse<DailyReport> { Items = items, TotalCount = totalCount, Page = query.Page, PageSize = query.PageSize };
        }

        public async Task<DailyReportDetailDto?> GetReportDetailAsync(int companyId, int id)
        {
            var report = await _repo.GetReportByIdAsync(companyId, id);
            if (report == null) return null;

            var photos = await _repo.GetPhotosByReportAsync(id);
            return new DailyReportDetailDto
            {
                Id = report.Id, ProjectId = report.ProjectId, ReportDate = report.ReportDate,
                Weather = report.Weather, WorkerCount = report.WorkerCount, Summary = report.Summary,
                Issues = report.Issues, CreatedByUserId = report.CreatedByUserId, CreatedAt = report.CreatedAt,
                Photos = photos.ToList()
            };
        }

        public async Task<DailyReport> CreateReportAsync(int companyId, int userId, CreateDailyReportDto dto)
        {
            var project = await _projectRepo.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null) throw new Exception("Project not found.");

            // Check for duplicate
            var existing = await _repo.GetReportByDateAsync(companyId, dto.ProjectId, dto.ReportDate);
            if (existing != null) throw new Exception($"Daily report for {dto.ReportDate:yyyy-MM-dd} already exists for this project.");

            var report = new DailyReport
            {
                ProjectId = dto.ProjectId, CompanyId = companyId, ReportDate = dto.ReportDate,
                Weather = dto.Weather, WorkerCount = dto.WorkerCount, Summary = dto.Summary,
                Issues = dto.Issues, CreatedByUserId = userId
            };

            var reportId = await _repo.CreateReportAsync(report);
            report.Id = reportId;

            // Add photos
            foreach (var photo in dto.Photos)
            {
                await _repo.AddPhotoAsync(new DailyReportPhoto { DailyReportId = reportId, ImageUrl = photo.ImageUrl, Caption = photo.Caption });
            }

            return report;
        }

        public async Task<DailyReport?> UpdateReportAsync(int companyId, int id, UpdateDailyReportDto dto)
        {
            var existing = await _repo.GetReportByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.Weather = dto.Weather;
            existing.WorkerCount = dto.WorkerCount;
            existing.Summary = dto.Summary;
            existing.Issues = dto.Issues;

            var success = await _repo.UpdateReportAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> DeleteReportAsync(int companyId, int id)
        {
            return await _repo.DeleteReportAsync(companyId, id);
        }
    }
}
