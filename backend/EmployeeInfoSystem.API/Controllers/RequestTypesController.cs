using EmployeeInfoSystem.Application.DTOs.RequestType;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestTypesController : BaseController
    {
        private readonly IRequestTypeService _requestTypeService;

        public RequestTypesController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }

        // GET api/requesttypes/active  -- employee: выпадающий список
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var list = await _requestTypeService.GetAllActiveAsync();
            return Ok(list);
        }

        // GET api/requesttypes  -- admin: полный список
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _requestTypeService.GetAllAsync();
            return Ok(list);
        }

        // POST api/requesttypes  -- admin
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateRequestTypeDto dto)
        {
            var result = await _requestTypeService.CreateAsync(dto);
            return CreatedFromResult(result, nameof(GetAll));
        }

        // PUT api/requesttypes/{id}  -- admin
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRequestTypeDto dto)
        {
            var result = await _requestTypeService.UpdateAsync(id, dto);
            return FromResult(result);
        }

        // DELETE api/requesttypes/{id}  -- admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _requestTypeService.DeleteAsync(id);
            return FromResult(result);
        }
    }
}
