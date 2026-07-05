using EmployeeInfoSystem.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeInfoSystem.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult FromResult(Result result) =>
            result.IsSuccess ? NoContent() : ErrorResponse(result.Error);

        protected IActionResult FromResult<T>(Result<T> result) =>
           result.IsSuccess ? Ok(result.Value): ErrorResponse(result.Error);

        protected IActionResult CreatedFromResult(Result<int> result, string actionName) =>
            result.IsSuccess
                ? CreatedAtAction(actionName, new { id = result.Value }, null)
                : ErrorResponse(result.Error);
        private IActionResult ErrorResponse(Error error) => error.Type switch
        {
            ErrorType.NotFound => NotFound(new { error = error.Message, type = error.Type.ToString() }),
            ErrorType.Conflict => Conflict(new { error = error.Message, type = error.Type.ToString() }),
            ErrorType.Validation => BadRequest(new { error = error.Message, type = error.Type.ToString() }),
            ErrorType.Unauthorized => Unauthorized(new { error = error.Message, type = error.Type.ToString() }),
            ErrorType.Forbidden => StatusCode(403, new { error = error.Message, type = error.Type.ToString() }),
            ErrorType.External => StatusCode(502, new { error = error.Message, type = error.Type.ToString() }),
            _ => StatusCode(500, new { error = error.Message, type = error.Type.ToString() })
        };
    }

}
