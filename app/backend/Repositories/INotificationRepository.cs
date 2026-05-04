using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface INotificationRepository
    {
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetNotificationsPaginatedAsync(int companyId, int userId, int offset, int pageSize);
        Task<int> GetUnreadCountAsync(int companyId, int userId);
        Task<int> CreateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int companyId, int id);
        Task<int> MarkAllAsReadAsync(int companyId, int userId);
    }
}
