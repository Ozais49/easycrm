# Multi-tenancy

> Purpose: how tenant data is isolated. Status: Draft. Decisions: [ADR-0002](../adr/0002-shared-schema-tenancy-with-rls.md), [ADR-0015](../adr/0015-tenant-resolution-and-provisioning.md)

## Model
- Shared database; each module has its own schema. Every tenant-owned table has `tenant_id uuid not null`.
- Clerk Organization ↔ `Tenant` (1:1, unique `clerk_org_id`). Users may belong to multiple orgs; the active org comes from the session token.
- `identity.tenants` and `identity.users` are global (no `tenant_id`, no RLS). `identity.tenant_memberships` is tenant-owned.

## Resolving the tenant
- **Requests**: `/api/*` is a route group with `TenantResolutionFilter` (Identity.Endpoints). It reads the user (`sub`) and organization from the Clerk token (v2 `o` claim `{id, rol, slg}`, v1 `org_id`/`org_slug`), resolves or provisions tenant, user and membership, then sets the scoped `RequestContext` (`ITenantContext`, `ICurrentUser`).
  - No organization in the token → 403 `tenant.required`. Membership not `Active` → 403 `tenant.membership_inactive`.
  - Resolved sessions are cached in memory for 60 seconds (bounded cache).
- **A scope belongs to one tenant**: setting a different tenant (or user) on the same scope throws.
- **Webhooks and background work** have no session: they set the tenant explicitly from the event before touching tenant data.

## Provisioning
Two paths converge on the same idempotent, race-safe code (`IdentityProvisioner`: find, else insert, on unique violation reload):
1. **Just-in-time**: the first valid token for an unknown org/user/membership creates them. Tenant name and slug fall back to the token slug (or org id); user email and names stay empty. Works locally before webhooks are reachable.
2. **Clerk webhooks** (`POST /webhooks/clerk`): `user.*`, `organization.*`, `organizationMembership.*` upsert the real details and soft-delete on `*.deleted` (membership → `Removed`). Events may arrive in any order or before a JIT row exists.

## Enforcement (defense in depth)
1. **EF named query filter `Tenant`** on every `ITenantEntity` root: `TenantId == current tenant`. **No tenant means no rows** (the filter compares against `Guid.Empty`).
2. **`TenantId` is stamped** from the context on insert (`TenantStampInterceptor`; no tenant → exception) and **immutable** afterwards (EF throws on a modified value). `CreatedBy`/`UpdatedBy` are stamped from `ICurrentUser`.
3. **Postgres RLS**: `TenantSessionConnectionInterceptor` runs `set_config('app.tenant_id', <id or ''>, false)` every time a connection is opened (connections are pooled, so the value must be overwritten each time). Policy `tenant_isolation` on each tenant table: `USING/WITH CHECK (tenant_id = NULLIF(current_setting('app.tenant_id', true), '')::uuid)`; unset or empty matches nothing. Add it in the table's migration with `migrationBuilder.EnableTenantRls(schema, table)`.
4. **DB roles**: migrations run as `cem_owner`; the API connects as `cem_app` (not owner, no `BYPASSRLS`). One-time setup: [deploy/sql/init-roles.sql](../../deploy/sql/init-roles.sql). Apply migrations with the owner connection: `dotnet ef database update ... --connection "<cem_owner connection string>"`.

## Non-user contexts
- Integration webhooks (WhatsApp, IG, TikTok): tenant resolved via `IntegrationAccount` (`phone_number_id`, IG account id, TikTok advertiser id).
- Background jobs / outbox: tenant id carried on each message.
- Platform admin: separate role/connection allowed to bypass — TODO design.

## TODO
- Seeding on tenant creation (roles, settings, picklists)
- Tenant deletion and data export
- Per-tenant limits (storage, users, integrations)
