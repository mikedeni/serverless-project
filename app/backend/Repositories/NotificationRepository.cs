using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DapperContext _context;
        public NotificationRepository(DapperContext context) { _context = context; }

        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetNotificationsPaginatedAsync(int companyId, int userId, int offset, int pageSize)
        {
            using var connection = _context.CreateConnection();
            var where = "WHERE CompanyId = @CompanyId AND UserId = @UserId";
            var count = await connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM Notifications {where};", new { CompanyId = companyId, UserId = userId });
            var items = await connection.QueryAsync<Notification>(
                $"SELECT * FROM Notifications {where} ORDER BY IsRead ASC, CreatedAt DESC LIMIT @PageSize OFFSET @Offset;",
                new { CompanyId = companyId, UserId = userId, PageSize = pageSize, Offset = offset });
            return (items, count);
        }

        public async Task<int> GetUnreadCountAsync(int companyId, int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Notifications WHERE CompanyId = @CompanyId AND UserId = @UserId AND IsRead = 0;",
                new { CompanyId = companyId, UserId = userId });
        }

        public async Task<int> CreateNotificationAsync(Notification notification)
        {
            using var connection = _context.CreateConnection();
            notification.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Notifications (UserId, CompanyId, Type, Title, Message, RelatedUrl, IsRead, CreatedAt)
                VALUES (@UserId, @CompanyId, @Type, @Title, @Message, @RelatedUrl, 0, @CreatedAt); SELECT LAST_INSERT_ID();", notification);
        }

        public async Task<bool> MarkAsReadAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Notifications SET IsRead = 1 WHERE Id = @Id AND CompanyId = @CompanyId;",
                new { Id = id, CompanyId = companyId }) > 0;
        }

        public async Task<int> MarkAllAsReadAsync(int companyId, int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Notifications SET IsRead = 1 WHERE CompanyId = @CompanyId AND UserId = @UserId AND IsRead = 0;",
                new { CompanyId = companyId, UserId = userId });
        }
    }
}
