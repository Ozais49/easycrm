# Backend Style Guide (C# / .NET 10)

> Purpose: consistent backend code. Status: Draft

## General
- File-scoped namespaces, nullable enabled, warnings as errors (TODO confirm)
- `.editorconfig` is the source of truth for formatting — TODO add
- One public type per file; file name = type name

## Naming
- PascalCase types/methods/properties; camelCase locals/params; `_camelCase` private fields
- Commands: `CreateContactCommand`, handlers: `CreateContactHandler`, queries: `GetContactByIdQuery`
- Endpoints grouped per module: `ContactEndpoints.Map(...)`
- Integration events past tense: `DealWon`, `LeadConverted`

## Patterns
- Handlers return `Result<T>` (no exceptions for expected failures); map to ProblemDetails at the edge
- Validation with FluentValidation via a Mediator pipeline behavior (TODO confirm library)
- Domain entities protect invariants (private setters, factory methods); no anemic setters for business rules
- Async all the way; pass `CancellationToken`
- No business logic in endpoints or EF configurations

## Logging
- Structured logging; never log secrets, tokens or sensitive field values
- Include `TenantId`, `UserId`, correlation id in scope

## TODO
- Mapping approach (manual vs Mapperly), date/time abstraction (`TimeProvider`), id generation (UUID v7)
