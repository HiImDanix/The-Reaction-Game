using Application;

namespace ReaktlyC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionHandler : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = context.Exception.Message;
        
        _logger.LogError(context.Exception, "An unhandled exception occurred");

        context.Result = new ObjectResult(new { error = message })
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }
}