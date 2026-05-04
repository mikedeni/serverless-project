using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Services;
using ConstructionSaaS.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] PaginationQuery query)
        {
            var companyId = User.GetCompanyId();
            var userId = User.GetUserId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            return Ok(await _service.GetNotificationsPaginatedAsync(companyId, userId, query));
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var companyId = User.GetCompanyId();
            var userId = User.GetUserId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var count = await _service.GetUnreadCountAsync(companyId, userId);
            return Ok(new { unreadCount = count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var success = await _service.MarkAsReadAsync(companyId, id);
            if (!success) return NotFound("Notification not found.");
            return Ok(new { message = "Marked as read." });
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var companyId = User.GetCompanyId();
            var userId = User.GetUserId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");
            var count = await _service.MarkAllAsReadAsync(companyId, userId);
            return Ok(new { message = $"Marked {count} notifications as read." });
        }
    }
}
