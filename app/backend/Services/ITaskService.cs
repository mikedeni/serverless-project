using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(int companyId, TaskItem taskData);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int companyId, int projectId);
        Task<bool> UpdateTaskStatusAsync(int companyId, int taskId, UpdateTaskStatusDto updateDto);
        Task<TaskUpdate> AddSiteUpdateAsync(int companyId, int taskId, CreateTaskUpdateDto siteUpdateDto);
        Task<bool> DeleteTaskAsync(int companyId, int taskId);
        Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(int companyId, int taskId);
    }
}
