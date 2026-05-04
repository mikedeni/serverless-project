using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync(int companyId)
        {
            return await _repository.GetProjectsByCompanyIdAsync(companyId);
        }

        public async Task<Project?> GetProjectByIdAsync(int companyId, int id)
        {
            return await _repository.GetProjectByIdAsync(companyId, id);
        }

        public async Task<Project> CreateProjectAsync(int companyId, Project projectData)
        {
            // Business Logic: Security override to guarantee the DB record matches the authenticated tenant
            projectData.CompanyId = companyId;
            
            var newId = await _repository.CreateProjectAsync(projectData);
            projectData.Id = newId;
            
            return projectData;
        }

        public async Task<Project?> UpdateProjectAsync(int companyId, int id, Project projectData)
        {
            var existing = await _repository.GetProjectByIdAsync(companyId, id);
            if (existing == null) return null;

            // Preserve tenant isolation
            existing.ProjectName = projectData.ProjectName;
            existing.StartDate = projectData.StartDate;
            existing.EndDate = projectData.EndDate;
            existing.Budget = projectData.Budget;
            existing.Status = projectData.Status;
            existing.ImageUrl = projectData.ImageUrl;

            var success = await _repository.UpdateProjectAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            return await _repository.UpdateImageUrlAsync(companyId, id, imageUrl);
        }

        public async Task<bool> DeleteProjectAsync(int companyId, int id)
        {
            return await _repository.DeleteProjectAsync(companyId, id);
        }

        public async Task<PaginatedResponse<Project>> GetProjectsPaginatedAsync(int companyId, PaginationQuery query)
        {
            var (items, totalCount) = await _repository.GetProjectsPaginatedAsync(
                companyId, query.Offset, query.PageSize, query.Search, query.Status);

            return new PaginatedResponse<Project>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
