# Database Guidelines (PostgreSQL)

> Purpose: schema and migration conventions. Status: Draft

## Naming
- snake_case tables and columns (EF naming convention plugin); plural table names
- Schema per module — TODO confirm (`crm`, `sales`, `projects`, ...)
- FKs `<entity>_id`; indexes `ix_<table>_<cols>`; unique `ux_...`

## Required columns
- `id uuid` (v7), `tenant_id uuid not null`, audit columns, `deleted_at` (soft delete)

## Tenancy & RLS
- Every tenant table: index starting with `tenant_id`; RLS enabled + policy in the same migration
- Uniqueness constraints include `tenant_id`

## Types
- Money `numeric(18,2)` (NPR only, no currency column); timestamps `timestamptz` (UTC); JSONB for custom fields
- Enums stored as text (readable, migration-friendly)

## Migrations
- EF Core migrations per module DbContext; reviewed in PR; never edit an applied migration
- Destructive changes in two steps (expand → contract)

## TODO
- Backup/restore strategy, seed data approach, full-text search (pg_trgm / tsvector)
