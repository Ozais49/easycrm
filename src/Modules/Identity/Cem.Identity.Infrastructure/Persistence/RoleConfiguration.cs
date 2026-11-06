using System.Text.Json;
using System.Text.Json.Serialization;
using Cem.Identity.Domain;
using Cem.SharedKernel.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    // The stored JSON format: camelCase text enums, same shape as the API.
    private static readonly JsonSerializerOptions MatrixJson = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name).HasMaxLength(100);

        // Read and edited as a whole, so one JSONB column rather than a row per permission (ADR-0016).
        builder.Property(r => r.Permissions)
            .HasColumnType("jsonb")
            .HasConversion(
                matrix => JsonSerializer.Serialize(matrix, MatrixJson),
                json => JsonSerializer.Deserialize<PermissionMatrix>(json, MatrixJson)!);

        builder.HasOne<Tenant>().WithMany().HasForeignKey(r => r.TenantId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.TenantId, r.Name }).IsUnique().HasDatabaseName("ux_roles_tenant_id_name");
    }
}
