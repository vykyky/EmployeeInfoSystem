using EmployeeInfoSystem.Application.DTOs.Auth;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/<AuthController>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            return FromResult(await _authService.LoginAsync(dto));
        }

    }
}
