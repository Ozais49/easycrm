# Backend Style Guide (C# / .NET 10)

> Purpose: consistent backend code. Status: Draft

## General
- File-scoped namespaces, nullable enabled, warnings as errors
- `.editorconfig` (repo root) is the source of truth for formatting; packages are versioned centrally in `Directory.Packages.props`
- One public type per file; file name = type name

## Naming
- PascalCase types/methods/properties; camelCase locals/params; `_camelCase` private fields
- Commands: `CreateContactCommand`, handlers: `CreateContactHandler`, queries: `GetContactByIdQuery`
- Endpoints grouped per module: `ContactEndpoints.Map(...)`
- Integration events past tense: `DealWon`, `LeadConverted`

## Patterns
- Handlers return `Result<T>` (no exceptions for expected failures); map to ProblemDetails at the edge
- Validation with FluentValidation (confirmed) via a Mediator pipeline behavior that returns a failed `Result` with field errors instead of throwing
- Domain entities protect invariants (private setters, factory methods); no anemic setters for business rules
- Async all the way; pass `CancellationToken`
- No business logic in endpoints or EF configurations

## Logging
- Structured logging; never log secrets, tokens or sensitive field values
- Include `TenantId`, `UserId`, correlation id in scope

## Mapping, time and ids
- Mapping is manual (projections / small mapping methods); no mapper library
- Inject `TimeProvider` for the current time; never call `DateTime.UtcNow` directly
- Entity ids are UUID v7 via `Guid.CreateVersion7()` (`Entity` base class)

Decisions: [ADR-0014](../adr/0014-backend-conventions.md)
