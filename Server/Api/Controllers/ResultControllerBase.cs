using System.Net;
using Application.Errors;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

namespace ReaktlyC.Controllers;

[ApiController]
public class ResultControllerBase : ControllerBase
{
    // private readonly ILogger<ResultControllerBase> _logger;
    //
    // public ResultControllerBase(ILogger<ResultControllerBase> logger)
    // {
    //     _logger = logger;
    // }

    protected IActionResult ResponseFromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value == null ? NoContent() : Ok(result.Value);
        }

        return ResponseFromErrors(result.Errors);
    }
    
    protected IActionResult ResponseFromResult(Result result)
    {
        return result.IsSuccess ? NoContent() : ResponseFromErrors(result.Errors);
    }

    protected IActionResult ResponseFromErrors(List<IError> errors)
    {
        if (errors.Count == 0)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        var primaryError = errors.FirstOrDefault();
        var statusCode = GetStatusCodeForError(primaryError);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = GetErrorTitle(statusCode),
            Type = GetErrorType(statusCode),
            Detail = primaryError?.Message,
            Extensions = 
            {
                ["errors"] = errors.Select(e => new ApiError
                {
                    Code = GetErrorCode(e),
                    Message = e.Message,
                    Details = e.Metadata
                }).ToList()
            }
        };

        // _logger.LogError("Errors occurred: {@ProblemDetails}", problemDetails);

        return StatusCode((int)statusCode, problemDetails);
    }

    private static HttpStatusCode GetStatusCodeForError(IError error)
    {
        return error switch
        {
            NotFoundError => HttpStatusCode.NotFound,
            BusinessValidationError => HttpStatusCode.UnprocessableEntity,
            UnauthorizedError => HttpStatusCode.Unauthorized,
            InvalidInputError => HttpStatusCode.BadRequest,
            ForbiddenError => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorCode(IError error)
    {
        return error switch
        {
            NotFoundError => "NOT_FOUND",
            BusinessValidationError => "BUSINESS_VALIDATION_ERROR",
            UnauthorizedError => "UNAUTHORIZED",
            InvalidInputError => "INVALID_INPUT",
            ForbiddenError => "FORBIDDEN",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private static string GetErrorTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.UnprocessableEntity => "Unprocessable Entity",
            _ => "Internal Server Error"
        };
    }

    private static string GetErrorType(HttpStatusCode statusCode)
    {
        return $"https://httpstatuses.com/{(int)statusCode}";
    }
}

public class ApiError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Details { get; set; }
}

public class ForbiddenError : Error { }