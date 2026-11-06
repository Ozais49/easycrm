# ADR-0014: Backend conventions

- Status: Accepted
- Date: 2026-11-06
- Deciders: Project owner

## Context
The backend scaffold needs a handful of small, cross-cutting choices settled once, under the memory limits of ADR-0009 (1 GB VPS shared with Postgres).

## Decision
- **Logging**: built-in `Microsoft.Extensions.Logging`; JSON console formatter in production, simple console in Development, scopes included. No Serilog.
- **Validation**: FluentValidation, run by a Mediator pipeline behavior that returns a failed `Result` (Validation error with per-field messages) instead of throwing.
- **Mapping**: manual; no mapper library.
- **OpenAPI**: built-in `Microsoft.AspNetCore.OpenApi` (`/openapi/v1.json`); no Swashbuckle.
- **Schema per module**: each module owns a Postgres schema (`crm`, `sales`, ...), including its `__ef_migrations_history` table.
- **Central package management**: versions pinned in `Directory.Packages.props`; shared build settings (warnings as errors, nullable) in `Directory.Build.props`.
- **Time**: inject `TimeProvider`; no direct `DateTime.UtcNow`.
- **Ids**: UUID v7 via `Guid.CreateVersion7()`.

## Consequences
- + Fewer dependencies and lower memory; consistent, reviewable conventions
- + Failures are values (`Result`), mapped to RFC 7807 only at the HTTP edge
- − Less out-of-the-box tooling than Serilog (sinks, enrichers) and Swashbuckle (UI); manual mapping is more boilerplate
- Follow-up: revisit logging if a central log sink is needed

## Alternatives considered
- Serilog — extra memory and dependencies for features not yet needed
- Swashbuckle — heavier; built-in OpenAPI is now first-party
- Mapperly / AutoMapper — mapping is small and explicit; AutoMapper is reflection-heavy
- Single shared schema — module ownership gets blurred
