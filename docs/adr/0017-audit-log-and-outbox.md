# ADR-0017: Audit log and transactional outbox

- Status: Accepted
- Date: 2026-11-06
- Deciders: Project owner

## Context
Two cross-cutting needs share the same shape: an audit trail of business changes ([security-and-permissions](../architecture/security-and-permissions.md)) and reliable delivery of integration events between modules ([ADR-0005](0005-integration-events-outbox.md)). Both must commit or roll back together with the change that caused them, must not make modules depend on each other, and must fit a 1 GB VPS ([ADR-0009](0009-cloudflare-plus-low-end-vps.md)).

## Decision
- **Shared platform tables, written in the module's transaction.** `platform.audit_entries` and `platform.outbox_messages` are mapped by every module context through `CemDbContextBase`, so a module writes them in its own SaveChanges. Only `PlatformDbContext` (in the building block `Cem.Platform`) migrates them; every other context excludes them with `ExcludeFromMigrations()`. `Cem.Platform` also owns the outbox dispatcher and the audit log endpoint.
- **Audit via a SaveChanges interceptor.** `AuditInterceptor` adds an entry for every added, modified or deleted `ITenantEntity` in the same save. `{ "property": { "old", "new" } }` holds all properties for Created, only the changed ones for Updated, nothing for Deleted (a soft delete is recorded as Deleted). Audit columns, `TenantId` and the key are left out; an update that changed nothing else writes no row. The interceptor runs before the tenant stamp so its own entries are stamped.
- **Redaction.** Properties tagged `[Sensitive]` record `{ "redacted": true }` instead of old/new: the audit log shows that the value changed, never the value, so reading the log cannot bypass field-level security.
- **Entity type = `schema.table`** (e.g. `identity.roles`). It is stable (a table rename is a deliberate migration), needs no pluralization rules and is unique across modules; the CLR name can be refactored freely.
- **`audit_entries` is tenant-owned with RLS. `outbox_messages` is deliberately not**: the dispatcher serves all tenants from one loop and must read their messages before it can act for any of them. It holds integration events only, and the application role is the only way in. Each message carries `tenant_id` (null if no tenant context existed yet, e.g. `TenantCreated`), and the dispatcher sets the tenant context of the handler scope from it.
- **Publishing.** `CemDbContextBase.AddIntegrationEvent` serializes the event (System.Text.Json) and stamps the tenant; modules expose it through their store port (`IIdentityStore.Publish`). There is no in-memory collector, so an event cannot outlive or precede its transaction.
- **Stable event names.** `IIntegrationEvent.EventType` (e.g. `identity.tenant_created`) is stored with each message and maps to the CLR type through the handler registrations, so renaming a class never breaks stored messages. Events are declared in `<Module>.Contracts`.
- **Delivery.** In-process `OutboxDispatcher` (hosted service), concurrency 1: loops at once while a batch of 20 was full; when idle it sleeps until `OutboxSignal` is notified (an EF interceptor fires it after a committed SaveChanges that inserted outbox messages), with a 1-minute fallback poll for due retries (changed 2026-11-09 from a 2 s poll). A batch is locked with `FOR UPDATE SKIP LOCKED` in a transaction; each message runs in its own DI scope. At-least-once, so **handlers must be idempotent**. An event with no subscriber is marked processed.
- **Failure, backoff, dead messages.** A failing message gets `attempts++`, a truncated `last_error` (type and message, no stack) and `next_attempt_at = now + 2^attempts` seconds (cap 1 h). After 10 attempts it gets `dead_at` and is never retried but kept for inspection (a `dead_at` column reads better than a sentinel `infinity`). Every failure is logged at Error. A dispatcher failure that is not a message failure (database down) is logged and retried with backoff (4 s doubling to 1 min); it never stops the host. `Outbox:Enabled=false` turns the dispatcher off.
- **Retention.** Processed messages older than 7 days are deleted by the dispatcher at most once an hour. Dead messages and audit entries are kept.
- **No job library.** Background work is a plain `BackgroundService` plus the outbox for anything that must be durable and retried. Hangfire and Quartz are not adopted: extra memory, their own schema and tables, and nothing we need yet. Revisit if scheduling (cron-like) or a job dashboard is needed.

## Consequences
- + Audit and events are atomic with the change; no new service or dependency
- + One place to query the audit trail; modules stay decoupled
- − Every module context carries two extra entity types, and the cross-schema tables need grants for `cem_app` on `platform` ([init-roles.sql](../../deploy/sql/init-roles.sql))
- − The audit table grows without a purge; partitioning or archiving is a later decision
- − Handlers run in the same process as the API; a slow handler delays other messages (concurrency 1)
- − A message whose event type is not registered is marked processed silently (a typo drops events)
- Follow-up: dead message alerting and a way to requeue them; audit retention

## Alternatives considered
- Audit in each handler — easy to forget and cannot see every property change
- Postgres triggers for audit — no user id or redaction, logic outside the codebase
- Outbox with RLS — the dispatcher would need a bypass role or one connection per tenant
- Hangfire / Quartz — heavy for a 1 GB VPS, see above
- A separate audit/outbox table per module — duplicates the dispatcher and the query
