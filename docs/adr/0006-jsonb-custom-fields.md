# ADR-0006: Admin-defined custom fields stored in JSONB

- Status: Accepted
- Date: 2026-11-06

## Context
Different tenants need extra fields on entities (e.g. "Shoot location", "Instagram handle") without code changes.

## Decision
Core fields stay strongly typed. Each customizable entity has a `custom_fields jsonb` column; definitions (type: text, number, date, select, checkbox; required; order) live in `CustomFieldDefinition` per tenant + entity. UI renders them automatically in forms, details and list columns.

## Consequences
- + Flexible per tenant, no migrations per field
- − Filtering/sorting on custom fields needs GIN/expression indexes; validation done in app
- Out of scope: custom entities and relationships (full Entity Manager) — v2+

## Alternatives considered
- EAV tables — slower, complex queries
- Fixed schema — not flexible enough for multi-tenant SaaS
