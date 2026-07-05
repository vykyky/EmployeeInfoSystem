using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : BaseController
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        // POST api/<SyncController>
        [HttpPost("profile")]
        public async Task<IActionResult> SyncProfile([FromQuery] string? tabn)
        {
            if (!string.IsNullOrEmpty(tabn))
            {
                if (!await _syncService.TabnExistsAsync(tabn))
                    return FromResult(Result.Failure(Error.NotFound($"Лицевой счет {tabn} не найден в Галактике")));

                await _syncService.SyncProfileByTabnAsync(tabn);
            }
            else
            {
                await _syncService.SyncAllProfilesAsync();
            }

            return FromResult(Result.Success());
        }

        // POST api/sync/ppe?tabn=12345  — один сотрудник
        // POST api/sync/ppe             — все сотрудники
        [HttpPost("ppe")]
        public async Task<IActionResult> SyncPpe([FromQuery] string? tabn)
        {
            if (!string.IsNullOrEmpty(tabn))
            {
                if (!await _syncService.TabnExistsAsync(tabn))
                    return FromResult(Result.Failure(Error.NotFound($"Лицевой счет {tabn} не найден в Галактике")));

                await _syncService.SyncPpeByTabnAsync(tabn);
            }
            else
            {
                await _syncService.SyncAllPpeAsync();
            }

            return FromResult(Result.Success());
        }

    }
}
