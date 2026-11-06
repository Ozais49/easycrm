# ADR-0001: Modular monolith with Clean layers per module

- Status: Accepted
- Date: 2026-11-06

## Context
Small team, free-tier hosting, but the product must grow into multi-tenant SaaS with distinct functional areas (CRM, Sales, Projects, Portfolio, Inbox).

## Decision
One deployable .NET 10 host. Each module has Domain / Application / Infrastructure / Endpoints / Contracts projects. Cross-module access only via Contracts or integration events. Boundaries enforced by convention and code review (no architecture tests for now — ADR-0010).

## Consequences
- + Simple deploy and ops; modules extractable later
- + Clear ownership and boundaries
- − More projects/ceremony than a single-project app
- Follow-up: add NetArchTest boundary tests when automated tests are introduced

## Alternatives considered
- Classic 4-project Clean Architecture — degrades as modules grow
- Vertical slice single project — weaker boundaries
- Microservices — far too much ops for the scale
