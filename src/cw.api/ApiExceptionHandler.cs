using CW.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CW.Api;

public class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is InputValidationException inputValidationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ValidationProblemDetails(inputValidationException.ValidationResults.ToDictionary(vr => vr.MemberNames.FirstOrDefault() ?? string.Empty, vr => new string[] { vr.ErrorMessage ?? string.Empty }))
                {
                    Title = "Input validation failed",
                    Status = StatusCodes.Status400BadRequest,
                },
            });
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = "Unhandled error",
                    Status = StatusCodes.Status500InternalServerError,
                },
            });
        }
    }
}