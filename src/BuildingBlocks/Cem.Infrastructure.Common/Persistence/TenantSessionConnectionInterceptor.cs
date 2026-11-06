using System.Data.Common;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>
/// Feeds the current tenant to Postgres row-level security by setting <c>app.tenant_id</c> on the connection.
/// </summary>
/// <remarks>
/// Runs on every connection open, not once per context: connections are pooled, so a connection returned to the
/// pool still carries the previous request's value until it is overwritten here. No tenant sets an empty string,
/// which the RLS policy treats as "match no rows".
/// </remarks>
public sealed class TenantSessionConnectionInterceptor(ITenantContext tenantContext) : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var command = CreateCommand(connection);
        command.ExecuteNonQuery();
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await using var command = CreateCommand(connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private DbCommand CreateCommand(DbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = "select set_config('app.tenant_id', @tenant, false)";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "tenant";
        parameter.Value = tenantContext.TenantId?.ToString() ?? string.Empty;
        command.Parameters.Add(parameter);

        return command;
    }
}
