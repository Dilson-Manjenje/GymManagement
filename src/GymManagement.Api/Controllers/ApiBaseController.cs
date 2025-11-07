using Microsoft.AspNetCore.Mvc;
using ErrorOr;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiBaseController : ControllerBase
{

  public IActionResult Problem(List<Error> errors)
  {
    if (errors.Count is 0)
      return Problem();

    if (errors.All(error => error.Type == ErrorType.Validation))
      return ValidationProblem(errors);

    return Problem(errors[0]);
  }

  public IActionResult ValidationProblem(List<Error> errors)
  {
    var modelStateDicitionary = new ModelStateDictionary();
    foreach (var error in errors)
    {
      modelStateDicitionary.AddModelError(error.Code, error.Description);
    }

    return ValidationProblem(modelStateDicitionary);
  }

  public IActionResult Problem(Error error)
  {
    var statusCode = error.Type switch
    {
      ErrorType.Conflict => StatusCodes.Status409Conflict,
      ErrorType.Validation => StatusCodes.Status400BadRequest,
      ErrorType.NotFound => StatusCodes.Status404NotFound,
      ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
      _ => StatusCodes.Status500InternalServerError
    };

    return Problem(statusCode: statusCode, detail: $"{error.Description}");
  }
}