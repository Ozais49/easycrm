using System;
using Cem.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cem.Crm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsAndCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "custom_fields",
                schema: "crm",
                table: "contacts",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "custom_fields",
                schema: "crm",
                table: "companies",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "company_tags",
                schema: "crm",
                columns: table => new
                {
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_tags", x => new { x.company_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_company_tags_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "crm",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_company_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "crm",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_tags",
                schema: "crm",
                columns: table => new
                {
                    contact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact_tags", x => new { x.contact_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_contact_tags_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "crm",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contact_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "crm",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_company_tags_tag_id",
                schema: "crm",
                table: "company_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_tags_tenant_id_tag_id",
                schema: "crm",
                table: "company_tags",
                columns: new[] { "tenant_id", "tag_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contact_tags_tag_id",
                schema: "crm",
                table: "contact_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_tags_tenant_id_tag_id",
                schema: "crm",
                table: "contact_tags",
                columns: new[] { "tenant_id", "tag_id" });

            // Tag names are unique per tenant ignoring case (an expression index, so EF cannot model it).
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX ux_tags_tenant_id_name_lower ON crm.tags (tenant_id, lower(name));");

            migrationBuilder.EnableTenantRls("crm", "tags");
            migrationBuilder.EnableTenantRls("crm", "company_tags");
            migrationBuilder.EnableTenantRls("crm", "contact_tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRls("crm", "contact_tags");
            migrationBuilder.DisableTenantRls("crm", "company_tags");
            migrationBuilder.DisableTenantRls("crm", "tags");

            migrationBuilder.DropTable(
                name: "company_tags",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "contact_tags",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "crm");

            migrationBuilder.DropColumn(
                name: "custom_fields",
                schema: "crm",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "custom_fields",
                schema: "crm",
                table: "companies");
        }
    }
}
