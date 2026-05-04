using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<TaskItem> CreateTaskAsync(int companyId, TaskItem taskData)
        {
            // Security: Ensure project belongs to company
            var project = await _projectRepository.GetProjectByIdAsync(companyId, taskData.ProjectId);
            if (project == null) throw new Exception("Project not found or unauthorized.");

            taskData.CompanyId = companyId;
            var newId = await _taskRepository.CreateTaskAsync(taskData);
            taskData.Id = newId;

            return taskData;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int companyId, int projectId)
        {
            return await _taskRepository.GetTasksByProjectIdAsync(companyId, projectId);
        }

        public async Task<bool> UpdateTaskStatusAsync(int companyId, int taskId, UpdateTaskStatusDto updateDto)
        {
            // Security check is done elegantly within the repository's WHERE clause
            return await _taskRepository.UpdateTaskStatusAsync(companyId, taskId, updateDto.Status);
        }

        public async Task<TaskUpdate> AddSiteUpdateAsync(int companyId, int taskId, CreateTaskUpdateDto siteUpdateDto)
        {
            var taskUpdate = new TaskUpdate
            {
                TaskId = taskId,
                Note = siteUpdateDto.Note,
                ImageUrl = siteUpdateDto.ImageUrl
            };

            var newId = await _taskRepository.CreateTaskUpdateAsync(companyId, taskUpdate);
            if (newId == 0) throw new Exception("Task not found or unauthorized to post updates.");

            taskUpdate.Id = newId;
            return taskUpdate;
        }

        public async Task<bool> DeleteTaskAsync(int companyId, int taskId)
        {
            return await _taskRepository.DeleteTaskAsync(companyId, taskId);
        }

        public async Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(int companyId, int taskId)
        {
            // Verify task belongs to company first
            var task = await _taskRepository.GetTaskByIdAsync(companyId, taskId);
            if (task == null) throw new Exception("Task not found or unauthorized.");

            return await _taskRepository.GetTaskUpdatesAsync(taskId);
        }
    }
}
