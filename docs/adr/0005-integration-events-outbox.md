# ADR-0005: Integration events via transactional outbox

- Status: Accepted
- Date: 2026-11-06

## Context
Modules must react to each other (e.g. DealWon → create Project) without tight coupling or lost events on crash.

## Decision
Modules write integration events to an `outbox_messages` table in the same transaction as their state change. A background dispatcher delivers them in-process to subscribers; handlers are idempotent.

## Consequences
- + Loose coupling, at-least-once delivery, survives restarts
- − Eventual consistency between modules; must design idempotent handlers
- Follow-up: dispatcher implementation, retry/poison handling, tenant id on each message

## Alternatives considered
- Direct calls via module Contracts — simpler, tighter coupling (still allowed for read queries)
