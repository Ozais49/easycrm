# Multi-tenancy

> Purpose: how tenant data is isolated. Status: Draft. Decision: [ADR-0002](../adr/0002-shared-schema-tenancy-with-rls.md)

## Model
- Shared database and schema; every tenant-owned table has `tenant_id uuid not null`.
- Clerk Organization ↔ `Tenant` (1:1, unique `clerk_org_id`). Users may belong to multiple orgs; the active org comes from the JWT.

## Enforcement (defense in depth)
1. **EF Core global query filter** on every `ITenantEntity`; `TenantId` stamped on insert and immutable after.
2. **Postgres RLS**: `USING (tenant_id = current_setting('app.tenant_id')::uuid)`; the app sets `app.tenant_id` per connection/transaction.
3. The app DB role is not the table owner and has no `BYPASSRLS`.

## Non-user contexts
- Webhooks: tenant resolved via `IntegrationAccount` (WhatsApp `phone_number_id`, IG account id, TikTok advertiser id).
- Background jobs / outbox: tenant id carried on each message.
- Platform admin: separate role/connection allowed to bypass — TODO design.

## TODO
- Tenant provisioning (Clerk org created → webhook → seed roles, settings, picklists)
- Tenant deletion and data export
- Per-tenant limits (storage, users, integrations)
