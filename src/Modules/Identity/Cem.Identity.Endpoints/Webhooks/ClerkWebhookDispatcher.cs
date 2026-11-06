using System.Text.Json;
using Cem.Identity.Application.Sync;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Endpoints.Webhooks;

/// <summary>Maps a Clerk webhook event to the matching Identity command. Unknown events are ignored.</summary>
public sealed class ClerkWebhookDispatcher(ISender sender)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    /// <exception cref="JsonException">The payload does not have the expected shape.</exception>
    public ValueTask<Result> DispatchAsync(string body, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<ClerkWebhookEnvelope>(body, JsonOptions)
            ?? throw new JsonException("Empty webhook payload.");

        return envelope.Type switch
        {
            "user.created" or "user.updated" => Send(envelope, (ClerkUserData u) =>
                new UpsertUserCommand(u.Id!, u.PrimaryEmail, u.FirstName, u.LastName, u.ImageUrl), cancellationToken),
            "user.deleted" => Send(envelope, (ClerkUserData u) => new DeleteUserCommand(u.Id!), cancellationToken),
            "organization.created" or "organization.updated" => Send(envelope, (ClerkOrganizationData o) =>
                new UpsertTenantCommand(o.Id!, o.Name ?? o.Slug ?? o.Id!, o.Slug ?? o.Id!), cancellationToken),
            "organization.deleted" => Send(envelope, (ClerkOrganizationData o) => new DeleteTenantCommand(o.Id!), cancellationToken),
            "organizationMembership.created" or "organizationMembership.updated" => Send(envelope, (ClerkMembershipData m) =>
                new UpsertMembershipCommand(m.Organization?.Id!, m.PublicUserData?.UserId!, m.Id!, m.Role == "org:admin"), cancellationToken),
            "organizationMembership.deleted" => Send(envelope, (ClerkMembershipData m) =>
                new RemoveMembershipCommand(m.Organization?.Id!, m.PublicUserData?.UserId!), cancellationToken),
            _ => ValueTask.FromResult(Result.Success())
        };
    }

    private ValueTask<Result> Send<TData, TCommand>(
        ClerkWebhookEnvelope envelope,
        Func<TData, TCommand> toCommand,
        CancellationToken cancellationToken)
        where TCommand : ICommand<Result>
    {
        var data = envelope.Data.Deserialize<TData>(JsonOptions) ?? throw new JsonException("Missing event data.");
        return sender.Send(toCommand(data), cancellationToken);
    }
}
