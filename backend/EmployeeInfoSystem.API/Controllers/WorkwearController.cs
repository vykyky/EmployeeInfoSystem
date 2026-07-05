using EmployeeInfoSystem.Application.DTOs.Workwear;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkwearController : BaseController
    {
        private readonly IWorkwearService _workwearService;

        public WorkwearController(IWorkwearService workwearService)
        {
            _workwearService = workwearService;
        }

        // GET: api/<WorkwearController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tabn = User.FindFirst("tabn")?.Value;
            if (string.IsNullOrEmpty(tabn))
                return Unauthorized(new { error = "Табельный номер не передан или пуст" }); ;

            var result = await _workwearService.GetWorkwearByTabnAsync(tabn);

            // Твой хелпер автоматически вернет Ok(Value) при успехе или правильный статус-код при ошибке
            return FromResult(result);
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestSizeChange([FromBody] ChangeSizesRequestDto dto)
        {
            var tabn = User.FindFirst("tabn")?.Value;
            if (string.IsNullOrEmpty(tabn))
                return Unauthorized(new { error = "Табельный номер не передан" });

            // Передаем DTO целиком!
            var result = await _workwearService.RequestSizeChangeAsync(tabn, dto);
            return FromResult(result);
        }
        //-----------------------------------------------
        //-----------------------------------------------
        //ОСТАЛЬНОЕ НЕ ИСПОЛЬЗУЕТСЯ

        // GET api/<WorkwearController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WorkwearController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WorkwearController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WorkwearController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
