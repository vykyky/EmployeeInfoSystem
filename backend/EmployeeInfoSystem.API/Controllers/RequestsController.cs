using EmployeeInfoSystem.Application.DTOs.Request;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : BaseController
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        // GET api/requests/my  -- employee: свои запросы
        [HttpGet("my")]
        [Authorize(Roles = "employee,manager,admin")]
        public async Task<IActionResult> GetMy()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var list = await _requestService.GetMyRequestsAsync(userId.Value);
            return Ok(list);
        }

        // POST api/requests  -- employee: создать запрос
        [HttpPost]
        [Authorize(Roles = "employee,manager,admin")]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _requestService.CreateAsync(userId.Value, dto);
            return CreatedFromResult(result, nameof(GetMy));
        }

        // GET api/requests  -- manager: свои; admin: все
        [HttpGet]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirst("role")?.Value;
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var list = role == "admin"
                ? await _requestService.GetAllAsync()
                : await _requestService.GetByManagerIdAsync(userId.Value);

            return Ok(list);
        }

        // PATCH api/requests/{id}/take  -- manager: взять в работу
        [HttpPatch("{id}/take")]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Take(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _requestService.TakeInProgressAsync(id, userId.Value);
            return FromResult(result);
        }

        // PATCH api/requests/{id}/complete  -- manager: завершить
        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Complete(int id, [FromBody] UpdateRequestStatusDto dto)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _requestService.CompleteAsync(id, userId.Value, dto.ResolutionComment ?? string.Empty);
            return FromResult(result);
        }

        // PATCH api/requests/{id}/assign  -- admin only: назначить менеджера
        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignManagerDto dto)
        {
            var result = await _requestService.AssignManagerAsync(id, dto.ManagerId);
            return FromResult(result);
        }

        private int? GetUserId()
        {
            var raw = User.FindFirst("userId")?.Value;
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
