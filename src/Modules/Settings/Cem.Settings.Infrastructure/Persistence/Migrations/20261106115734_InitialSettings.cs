using System;
using Cem.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cem.Settings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "settings");

            migrationBuilder.CreateTable(
                name: "tenant_settings",
                schema: "settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_zone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    date_display = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    brand_color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_settings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_tenant_settings_tenant_id",
                schema: "settings",
                table: "tenant_settings",
                column: "tenant_id",
                unique: true);

            migrationBuilder.EnableTenantRls("settings", "tenant_settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRls("settings", "tenant_settings");

            migrationBuilder.DropTable(
                name: "tenant_settings",
                schema: "settings");
        }
    }
}
