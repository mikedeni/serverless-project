using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsAsync(int companyId);
        Task<Project?> GetProjectByIdAsync(int companyId, int id);
        Task<Project> CreateProjectAsync(int companyId, Project projectData);
        Task<Project?> UpdateProjectAsync(int companyId, int id, Project projectData);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
        Task<bool> DeleteProjectAsync(int companyId, int id);
        Task<PaginatedResponse<Project>> GetProjectsPaginatedAsync(int companyId, PaginationQuery query);
    }
}
