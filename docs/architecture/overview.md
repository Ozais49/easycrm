# Architecture Overview

> Purpose: the big picture. Status: Draft

## Shape
```
[Browser: React SPA] --Clerk JWT--> [Cem.Host (.NET 10)]
                                        |-- Modules (Crm, Calendar, Sales, Projects, Portfolio, Inbox, Settings, Identity)
                                        |-- Outbox dispatcher (background)
                                        |-- Webhooks (/webhooks/meta, /webhooks/tiktok, /webhooks/clerk)
                                        v
                                  [PostgreSQL (RLS)]   [Object storage (R2 / local)]
[Clerk] <-- JWKS + webhooks (user & org sync)
[Meta / TikTok APIs] <-- outbound send / inbound webhooks
```
Single deployable modular monolith — see [ADR-0001](../adr/0001-modular-monolith.md).

## Request pipeline
1. Clerk JWT validated (JWKS) → user + active org claims
2. Tenant resolution: Clerk org id → `TenantId` (`ITenantContext`); Postgres session var set for RLS
3. Permission resolution: effective role permissions (cached per user+tenant)
4. Endpoint → Mediator command/query → pipeline behaviors (validation, permission, transaction, outbox)
5. Response serialization strips `[Sensitive]` fields the caller cannot read

## Cross-cutting
- Errors: RFC 7807 ProblemDetails
- Logging: built-in `Microsoft.Extensions.Logging` with the JSON console formatter (simple console in Development); no Serilog — [ADR-0014](../adr/0014-backend-conventions.md)
- Real-time: TODO (SignalR for inbox/notifications assumed)
- Background jobs: outbox dispatcher, token refresh, reminders — TODO library choice
