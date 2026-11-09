using System;
using Cem.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cem.Crm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCrm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "crm");

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    website = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    pan_number = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_province = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    job_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    mobile = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_province = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contacts_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "crm",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_companies_tenant_id_name",
                schema: "crm",
                table: "companies",
                columns: new[] { "tenant_id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_companies_tenant_id_owner_user_id",
                schema: "crm",
                table: "companies",
                columns: new[] { "tenant_id", "owner_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_companies_tenant_id_team_id",
                schema: "crm",
                table: "companies",
                columns: new[] { "tenant_id", "team_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contacts_company_id",
                schema: "crm",
                table: "contacts",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_contacts_tenant_id_company_id",
                schema: "crm",
                table: "contacts",
                columns: new[] { "tenant_id", "company_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contacts_tenant_id_owner_user_id",
                schema: "crm",
                table: "contacts",
                columns: new[] { "tenant_id", "owner_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contacts_tenant_id_team_id",
                schema: "crm",
                table: "contacts",
                columns: new[] { "tenant_id", "team_id" });

            migrationBuilder.EnableTenantRls("crm", "companies");
            migrationBuilder.EnableTenantRls("crm", "contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRls("crm", "contacts");
            migrationBuilder.DisableTenantRls("crm", "companies");

            migrationBuilder.DropTable(
                name: "contacts",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "crm");
        }
    }
}
