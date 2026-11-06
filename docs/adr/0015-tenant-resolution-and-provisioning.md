# ADR-0015: Tenant resolution and provisioning

- Status: Accepted
- Date: 2026-11-06
- Deciders: Project owner

## Context
Every request needs a trusted tenant and user, backed by Postgres RLS ([ADR-0002](0002-shared-schema-tenancy-with-rls.md)). Clerk ([ADR-0003](0003-clerk-for-authentication.md)) is the source of truth for orgs and users, but webhooks are not reachable in local development and can arrive late or out of order.

## Decision
- **Resolution**: an endpoint filter on the `/api` group reads the Clerk session token (`sub`, and the org from the v2 `o` claim or v1 `org_id`), resolves tenant, user and membership, and fills a scoped `RequestContext`. A scope can hold only one tenant and one user.
- **Provisioning**: just-in-time from the token, plus Clerk webhooks (Svix-signed, verified manually) that fill in real details and handle deletions. Both use the same find-or-insert code that survives concurrent inserts via the unique indexes.
- **RLS**: the tenant id is passed to Postgres with `set_config('app.tenant_id', ..., false)` on every connection open (pooling). Policies use `NULLIF(current_setting('app.tenant_id', true), '')::uuid`, so an unset value matches no rows instead of erroring or matching all.
- **Roles**: migrations run as an owner role; the app uses a non-owner role without `BYPASSRLS`.
- **Placeholders**: JIT users have no email and tenants use the slug as name until the webhook arrives.
- **Sessions** are cached 60 seconds in a size-limited memory cache.

## Consequences
- + Works locally without webhooks; webhooks can arrive in any order
- + No tenant means no data, at the EF, stamping and database layers
- − Removed members keep access up to 60 s (the cache TTL)
- − One extra statement (`set_config`) per connection open
- − Two connection strings in production (owner for migrations, app for runtime)

## Alternatives considered
- Webhooks only — local development blocked until a tunnel exists; races with the first request
- `SET LOCAL` per transaction — requires wrapping every operation in a transaction
- A Clerk Backend API call per unknown user — extra dependency and latency; webhooks already deliver the data
