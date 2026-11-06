using Cem.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Cem.Infrastructure.Common.Http;

/// <summary>Maps handler results to HTTP responses (RFC 7807 for failures).</summary>
public static class ResultExtensions
{
    /// <summary>200 with the value, or the problem response.</summary>
    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error.ToHttpResult();

    /// <summary>204, or the problem response.</summary>
    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? TypedResults.NoContent() : result.Error.ToHttpResult();

    public static IResult ToHttpResult(this Error error)
    {
        var extensions = new Dictionary<string, object?> { ["code"] = error.Code };

        if (error.Type == ErrorType.Validation && error.FieldErrors is not null)
        {
            return TypedResults.ValidationProblem(
                error.FieldErrors.ToDictionary(e => e.Key, e => e.Value),
                detail: error.Message,
                extensions: extensions);
        }

        return TypedResults.Problem(
            detail: error.Message,
            statusCode: error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            },
            extensions: extensions);
    }
}
