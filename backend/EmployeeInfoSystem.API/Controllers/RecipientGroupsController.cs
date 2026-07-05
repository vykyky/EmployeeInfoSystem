using EmployeeInfoSystem.Application.DTOs.RecipientGroup;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipientGroupsController : BaseController
    {
        private readonly IRecipientGroupService _recipientGroupService;

        public RecipientGroupsController(IRecipientGroupService recipientGroupService)
        {
            _recipientGroupService = recipientGroupService;
        }

        // GET api/recipientgroups  -- все роли (employee для выбора, admin для управления)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _recipientGroupService.GetAllAsync();
            return Ok(list);
        }

        // POST api/recipientgroups  -- admin only
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateRecipientGroupDto dto)
        {
            var result = await _recipientGroupService.CreateAsync(dto);
            return CreatedFromResult(result, nameof(GetAll));
        }

        // DELETE api/recipientgroups/{id}  -- admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipientGroupService.DeleteAsync(id);
            return FromResult(result);
        }
    }
}
