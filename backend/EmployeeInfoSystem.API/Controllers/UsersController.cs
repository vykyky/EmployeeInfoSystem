using EmployeeInfoSystem.Application.DTOs.User;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return FromResult(await _service.GetByIdAsync(id));
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedFromResult(result, nameof(GetById));
        }

        // PUT api/<UsersController>/5




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return FromResult(await _service.DeleteAsync(id));
        }

        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] string role)
        {
            return FromResult(await _service.ChangeRoleAsync(id, role));
        }
    }
}
