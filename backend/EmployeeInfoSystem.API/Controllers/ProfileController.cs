using EmployeeInfoSystem.Application.DTOs.Profile;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: api/<ProfileController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tabn = User.FindFirst("tabn")?.Value;
            if (string.IsNullOrEmpty(tabn))
                return Unauthorized(new { error = "Табельный номер не передан или пуст" }); ;

            var result = await _profileService.GetProfileByTabnAsync(tabn);

            // Твой хелпер автоматически вернет Ok(Value) при успехе или правильный статус-код при ошибке
            return FromResult(result);
        }

        // POST: api/profile/request
        // Сотрудник отправляет запрос на изменение телефона/email
        [HttpPost("request")]
        public async Task<IActionResult> RequestContactChange([FromBody] ChangeContactsRequestDto dto)
        {
            var tabn = User.FindFirst("tabn")?.Value;
            if (string.IsNullOrEmpty(tabn))
                return Unauthorized(new { error = "Табельный номер не передан" });

            var result = await _profileService.RequestContactChangeAsync(tabn, dto);
            return FromResult(result);
        }
        //-----------------------------------------------
        //-----------------------------------------------
        //ОСТАЛЬНОЕ НЕ ИСПОЛЬЗУЕТСЯ

        // GET api/<ProfileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProfileController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProfileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
