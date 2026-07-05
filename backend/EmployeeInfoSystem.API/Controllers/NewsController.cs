using EmployeeInfoSystem.Application.DTOs.News;
using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInfoSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : BaseController
    {
        private readonly INewsService _service;
        private readonly IFileStorageService _fileStorage;
        public NewsController(INewsService service, IFileStorageService fileStorage) { 
            _service = service;
            _fileStorage = fileStorage;
        }

        // GET: api/<NewsController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET api/<NewsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return FromResult(await _service.GetByIdAsync(id));
        }

        // POST api/<NewsController>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateNewsDto dto, IFormFile? image)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Пользователь не распознан");
            }

            if (image != null)
                dto.ImagePath = await _fileStorage.SaveAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    "news"
                );

            var result = await _service.CreateAsync(dto, userId);
            return CreatedFromResult(result, nameof(GetById));
        }

        // PUT api/<NewsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateNewsDto dto, IFormFile? image)
        {
            if (id != dto.Id) return BadRequest(new { error = "Id в маршруте и в теле запроса не совпадают" });

            if (image != null)
                dto.ImagePath = await _fileStorage.SaveAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    "news"
                );

            return FromResult(await _service.UpdateAsync(dto));
        }

        // DELETE api/<NewsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return FromResult(await _service.DeleteAsync(id));
        }
    }
}
