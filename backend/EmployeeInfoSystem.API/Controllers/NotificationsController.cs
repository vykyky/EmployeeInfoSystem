using EmployeeInfoSystem.Application.DTOs.Notification;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET api/notifications/my  -- employee: обычные уведомления (RequestId == null)
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var list = await _notificationService.GetMyNotificationsAsync(userId.Value);
            return Ok(list);
        }

        // GET api/notifications/tasks  -- manager/admin: задачи (RequestId != null)
        [HttpGet("tasks")]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> GetTasks()
        {
            var role = User.FindFirst("role")?.Value;
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var list = role == "admin"
                ? await _notificationService.GetAllTasksAsync()
                : await _notificationService.GetMyTasksAsync(userId.Value);

            return Ok(list);
        }

        // PATCH api/notifications/{id}/read  -- отметить прочитанным
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _notificationService.MarkAsReadAsync(id, userId.Value);
            return FromResult(result);
        }

        // POST api/notifications/send  -- самостоятельная рассылка (Рис. 5-7)
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationDto dto)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _notificationService.SendAsync(userId.Value, dto);
            return FromResult(result);
        }

        private int? GetUserId()
        {
            var raw = User.FindFirst("userId")?.Value;
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
