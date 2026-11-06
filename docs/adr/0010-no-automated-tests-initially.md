# ADR-0010: No automated tests initially; manual verification per feature

- Status: Accepted
- Date: 2026-11-06

## Context
Speed of early delivery is prioritized. Each feature is verified manually by the product owner before deploy.

## Decision
No automated test projects (unit, integration, architecture or E2E) for now. Features are delivered one at a time, manually verified, deployed, and given status/feedback.

## Consequences
- + Faster initial delivery, smaller repo, no CI test infrastructure
- − Higher regression risk — especially tenant isolation and permission leaks
- Mitigation: code-review checklist in [src/CLAUDE.md](../../src/CLAUDE.md) covers tenancy, permissions and sensitive fields; RLS acts as a DB-level safety net
- Revisit: before onboarding a second tenant, add isolation/permission integration tests

## Alternatives considered
- Mandatory tests per feature — deferred
