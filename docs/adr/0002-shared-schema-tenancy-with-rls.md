# ADR-0002: Shared-schema multi-tenancy with TenantId + Postgres RLS

- Status: Accepted
- Date: 2026-11-06

## Context
Starts with one org, must support many client orgs cheaply on free-tier Postgres.

## Decision
Single database/schema; `tenant_id` on every tenant-owned row; EF Core global query filters plus Postgres Row-Level Security as a second safety net.

## Consequences
- + Cheapest, single migration path, works on Neon/Supabase free tiers
- + RLS protects against a missed filter or raw SQL
- − Noisy-neighbour risk; per-tenant backup/restore is harder
- Follow-up: connection-level `app.tenant_id` setting, platform-admin bypass design

## Alternatives considered
- Schema per tenant — N migrations, messy on free tiers
- Database per tenant — strongest isolation, too costly
