using Microsoft.AspNetCore.Mvc;
using ErrorOr;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiBaseController : ControllerBase
{

  protected IActionResult HandleErrors(List<Error> errors)
  {
    if (errors is null || errors.Count == 0)
      return base.Problem(); 

    if (errors.All(error => error.Type == ErrorType.Validation))
      return ValidationProblem(errors);

    var first = errors[0];

    return base.Problem(
        title: first.Code,
        detail: $"{first.Description}",
        statusCode: MapStatusCode(first.Type));
  }

  [NonAction]
  protected IActionResult ValidationProblem(List<Error> errors)
  {
    var modelState = new ModelStateDictionary();
    foreach (var error in errors)
    {
      modelState.AddModelError(error.Code, error.Description);
    }

    return base.ValidationProblem(modelState);
  }
  private static int MapStatusCode(ErrorType type) => type switch
  {
    ErrorType.Conflict => StatusCodes.Status409Conflict,
    ErrorType.Validation => StatusCodes.Status400BadRequest,
    ErrorType.NotFound => StatusCodes.Status404NotFound,
    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
    _ => StatusCodes.Status500InternalServerError
  };
}