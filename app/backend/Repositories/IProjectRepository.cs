using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetProjectsByCompanyIdAsync(int companyId);
        Task<Project?> GetProjectByIdAsync(int companyId, int id);
        Task<int> CreateProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int companyId, int id);
        Task<(IEnumerable<Project> Items, int TotalCount)> GetProjectsPaginatedAsync(int companyId, int offset, int pageSize, string? search, string? status);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
    }
}
