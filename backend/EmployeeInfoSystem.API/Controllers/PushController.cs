using EmployeeInfoSystem.Application.DTOs.PushSubscription;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeInfoSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PushController : BaseController
    {
        private readonly IPushNotificationService _pushService;

        public PushController(IPushNotificationService pushService)
        {
            _pushService = pushService;
        }

        [HttpGet("vapid-public-key")]
        [AllowAnonymous]
        public IActionResult GetVapidPublicKey()
        {
            var result = _pushService.GetVapidPublicKey();
            return FromResult(result); // Возвращает 200 OK со строкой ключа
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _pushService.SubscribeAsync(userId, dto, cancellationToken);
            return FromResult(result); // Возвращает 204 No Content
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] string endpoint, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var result = await _pushService.UnsubscribeAsync(userId, endpoint, cancellationToken);
            return FromResult(result); // Возвращает 204 No Content
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("userId")?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
}
}
