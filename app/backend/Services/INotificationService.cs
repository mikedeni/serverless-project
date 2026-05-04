using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface INotificationService
    {
        Task<PaginatedResponse<Notification>> GetNotificationsPaginatedAsync(int companyId, int userId, PaginationQuery query);
        Task<int> GetUnreadCountAsync(int companyId, int userId);
        Task<bool> MarkAsReadAsync(int companyId, int id);
        Task<int> MarkAllAsReadAsync(int companyId, int userId);
        Task<Notification> CreateNotificationAsync(int companyId, CreateNotificationDto dto);
    }
}
