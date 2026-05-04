using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface ITaskRepository
    {
        Task<int> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> GetTaskByIdAsync(int companyId, int taskId);
        Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int companyId, int projectId);
        Task<bool> UpdateTaskStatusAsync(int companyId, int taskId, string status);
        Task<int> CreateTaskUpdateAsync(int companyId, TaskUpdate update);
        Task<bool> DeleteTaskAsync(int companyId, int taskId);
        Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(int taskId);
    }
}
