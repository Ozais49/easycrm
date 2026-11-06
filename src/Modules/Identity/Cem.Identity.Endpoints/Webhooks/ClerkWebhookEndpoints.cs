using Cem.Identity.Endpoints.Webhooks;
using Cem.Infrastructure.Common.Auth;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Cem.Identity.Endpoints;

public static class ClerkWebhookEndpoints
{
    public static IEndpointRouteBuilder MapClerkWebhook(this IEndpointRouteBuilder app)
    {
        // Svix payloads are small; cap the body so an oversized request is rejected before it is read.
        app.MapPost("/webhooks/clerk", HandleAsync)
            .AllowAnonymous()
            .ExcludeFromDescription()
            .WithMetadata(new RequestSizeLimitAttribute(256 * 1024));
        return app;
    }

    private static async Task<IResult> HandleAsync(
        HttpRequest request,
        IOptions<ClerkOptions> options,
        ClerkWebhookDispatcher dispatcher,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // The signature covers the exact bytes, so read the raw body once and parse it only after verification.
        string body;
        try
        {
            using var reader = new StreamReader(request.Body);
            body = await reader.ReadToEndAsync(cancellationToken);
        }
        catch (BadHttpRequestException ex) when (ex.StatusCode == StatusCodes.Status413PayloadTooLarge)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status413PayloadTooLarge, detail: "Webhook body too large.");
        }

        var valid = request.Headers.TryGetValue("svix-id", out var id)
            && request.Headers.TryGetValue("svix-timestamp", out var timestamp)
            && request.Headers.TryGetValue("svix-signature", out var signature)
            && SvixSignatureVerifier.IsValid(
                options.Value.WebhookSigningSecret, id.ToString(), timestamp.ToString(), signature.ToString(), body,
                timeProvider.GetUtcNow());

        if (!valid)
        {
            return Error.Validation("webhook.invalid_signature", "Invalid webhook signature.").ToHttpResult();
        }

        try
        {
            var result = await dispatcher.DispatchAsync(body, cancellationToken);
            return result.IsSuccess ? TypedResults.Ok() : result.Error.ToHttpResult();
        }
        catch (JsonException)
        {
            return Error.Validation("webhook.invalid_payload", "Unreadable webhook payload.").ToHttpResult();
        }
    }
}
