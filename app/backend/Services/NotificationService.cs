using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaginatedResponse<Notification>> GetNotificationsPaginatedAsync(int companyId, int userId, PaginationQuery query)
        {
            var (items, totalCount) = await _repo.GetNotificationsPaginatedAsync(companyId, userId, query.Offset, query.PageSize);
            return new PaginatedResponse<Notification> { Items = items, TotalCount = totalCount, Page = query.Page, PageSize = query.PageSize };
        }

        public async Task<int> GetUnreadCountAsync(int companyId, int userId) => await _repo.GetUnreadCountAsync(companyId, userId);

        public async Task<bool> MarkAsReadAsync(int companyId, int id) => await _repo.MarkAsReadAsync(companyId, id);

        public async Task<int> MarkAllAsReadAsync(int companyId, int userId) => await _repo.MarkAllAsReadAsync(companyId, userId);

        public async Task<Notification> CreateNotificationAsync(int companyId, CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId, CompanyId = companyId, Type = dto.Type,
                Title = dto.Title, Message = dto.Message, RelatedUrl = dto.RelatedUrl
            };
            notification.Id = await _repo.CreateNotificationAsync(notification);
            return notification;
        }
    }
}
