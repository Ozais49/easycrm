# ADR-0004: martinothamar/Mediator for CQRS

- Status: Accepted
- Date: 2026-11-06

## Context
Want command/query handlers with pipeline behaviors (validation, permissions, transactions). MediatR moved to a commercial license in 2025.

## Decision
Use `Mediator` (martinothamar) — source-generated, MIT licensed, MediatR-like API.

## Consequences
- + No license cost, fast (no reflection), familiar patterns
- − Smaller community than MediatR

## Alternatives considered
- MediatR — commercial license
- Wolverine — more opinionated, heavier
- Hand-rolled handlers — more boilerplate
