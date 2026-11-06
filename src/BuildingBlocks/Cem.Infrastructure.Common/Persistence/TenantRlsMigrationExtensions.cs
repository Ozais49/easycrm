using Microsoft.EntityFrameworkCore.Migrations;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>
/// Row-level security helpers for migrations. Call for every tenant-owned table in the same migration that
/// creates it. Global tables (e.g. identity.tenants, identity.users) have no RLS.
/// </summary>
public static class TenantRlsMigrationExtensions
{
    private const string PolicyName = "tenant_isolation";

    // An unset or empty app.tenant_id becomes NULL, and tenant_id = NULL is never true: no tenant, no rows.
    private const string TenantMatch = "tenant_id = NULLIF(current_setting('app.tenant_id', true), '')::uuid";

    public static MigrationBuilder EnableTenantRls(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        migrationBuilder.Sql(
            $"""
            ALTER TABLE "{schema}"."{table}" ENABLE ROW LEVEL SECURITY;
            CREATE POLICY {PolicyName} ON "{schema}"."{table}"
                USING ({TenantMatch})
                WITH CHECK ({TenantMatch});
            """);
        return migrationBuilder;
    }

    public static MigrationBuilder DisableTenantRls(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        migrationBuilder.Sql(
            $"""
            DROP POLICY {PolicyName} ON "{schema}"."{table}";
            ALTER TABLE "{schema}"."{table}" DISABLE ROW LEVEL SECURITY;
            """);
        return migrationBuilder;
    }
}
