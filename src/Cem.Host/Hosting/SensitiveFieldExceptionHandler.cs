using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Microsoft.AspNetCore.Diagnostics;

namespace Cem.Host.Hosting;

/// <summary>Turns a rejected sensitive-field write into 403 <c>field.forbidden</c>. Handles that exception only.</summary>
public sealed class SensitiveFieldExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not SensitiveFieldWriteException)
        {
            return false;
        }

        await Error.Forbidden("field.forbidden", exception.Message).ToHttpResult().ExecuteAsync(httpContext);
        return true;
    }
}
