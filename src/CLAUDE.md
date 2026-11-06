# Backend — Claude Instructions

> Status: Draft. Read with [root CLAUDE.md](../CLAUDE.md) and [style-guide-backend](../docs/guides/style-guide-backend.md).

## Layout (planned)
```
src/
  Cem.Host/                      # ASP.NET Core host, DI wiring, middleware, OpenAPI
  BuildingBlocks/
    Cem.SharedKernel/            # Entity base, ITenantEntity, Result, errors, permission primitives
    Cem.Infrastructure.Common/   # EF base context, outbox, tenancy, Clerk auth, storage
  Modules/
    <Module>/
      <Module>.Domain/           # entities, value objects, domain events — no EF, no ASP.NET
      <Module>.Application/      # commands/queries (Mediator), validators, DTOs
      <Module>.Infrastructure/   # EF configurations, DbContext/schema, repositories
      <Module>.Endpoints/        # minimal API endpoint groups
      <Module>.Contracts/        # public: integration events + query interfaces for other modules
```
Modules: Identity/Tenancy, Crm (Companies, Contacts, Activities), Calendar, Sales, Projects (Deliverables), Portfolio, Inbox, Settings. See [modules.md](../docs/architecture/modules.md).

## Layering rules
- Domain → nothing. Application → Domain + SharedKernel. Infrastructure → Application. Endpoints → Application.
- Cross-module: only `*.Contracts` or integration events (outbox). Enforced by code review (no automated tests for now — ADR-0010).

## Checklist for every new feature
- [ ] Entity implements `ITenantEntity` (if tenant-owned) and has RLS policy in migration
- [ ] Command/query has a validator
- [ ] Endpoint declares required scope permission (`RequirePermission(Scope.X, Action.Y)`)
- [ ] Sensitive DTO properties tagged `[Sensitive(FieldGroup.Pricing|Financial|ContactPii)]`
- [ ] Audit-worthy changes emit audit entries
- [ ] Cross-module side effects go through integration events, not direct calls
- [ ] Manual self-check per [testing-guide](../docs/guides/testing-guide.md) (no automated tests for now)
- [ ] No new heavy dependency; memory impact noted in PR (ADR-0009)
- [ ] Reads use `AsNoTracking` + projections; lists paginated; no unbounded queries

## Commands
```bash
dotnet tool restore                      # once: installs dotnet-ef
dotnet build                             # warnings are errors
dotnet run --project src/Cem.Host        # http://localhost:5080  (/health, /openapi/v1.json in Development)
# new migration for a module context (replace placeholders):
dotnet ef migrations add <Name> --project src/Modules/<Module>/<Module>.Infrastructure --startup-project src/Cem.Host --context <Module>DbContext
dotnet ef database update --project src/Modules/<Module>/<Module>.Infrastructure --startup-project src/Cem.Host --context <Module>DbContext
```
Deploy: TODO.
