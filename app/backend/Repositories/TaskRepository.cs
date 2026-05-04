using System.Data;
using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DapperContext _context;

        public TaskRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateTaskAsync(TaskItem task)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Tasks (ProjectId, CompanyId, AssignedUserId, Title, Status, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @AssignedUserId, @Title, @Status, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            task.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, task);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int companyId, int taskId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Tasks WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<TaskItem>(sql, new { CompanyId = companyId, Id = taskId });
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int companyId, int projectId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Tasks WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId ORDER BY CreatedAt DESC;";
            return await connection.QueryAsync<TaskItem>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<bool> UpdateTaskStatusAsync(int companyId, int taskId, string status)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Tasks SET Status = @Status WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Status = status, Id = taskId, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<int> CreateTaskUpdateAsync(int companyId, TaskUpdate update)
        {
            using var connection = _context.CreateConnection();
            
            // Validates that the task belongs to the company gracefully
            var sql = @"
                INSERT INTO TaskUpdates (TaskId, Note, ImageUrl, CreatedAt)
                SELECT @TaskId, @Note, @ImageUrl, @CreatedAt
                FROM Tasks 
                WHERE Id = @TaskId AND CompanyId = @CompanyId;
                SELECT LAST_INSERT_ID();";
            
            update.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, 
                new { 
                    update.TaskId, 
                    update.Note, 
                    update.ImageUrl, 
                    update.CreatedAt, 
                    CompanyId = companyId 
                });
        }

        public async Task<bool> DeleteTaskAsync(int companyId, int taskId)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Tasks WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = taskId, CompanyId = companyId });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(int taskId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM TaskUpdates WHERE TaskId = @TaskId ORDER BY CreatedAt DESC;";
            return await connection.QueryAsync<TaskUpdate>(sql, new { TaskId = taskId });
        }
    }
}
