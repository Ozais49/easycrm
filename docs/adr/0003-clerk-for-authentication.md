# ADR-0003: Clerk for authentication

- Status: Accepted
- Date: 2026-11-06

## Context
Need sign-in, invites, MFA and an org/tenant concept without building identity ourselves.

## Decision
Use Clerk. Clerk Organizations map 1:1 to tenants. The API validates Clerk JWTs via JWKS and syncs users/orgs/memberships through Clerk webhooks. Roles, scope permissions and field-level permissions stay in our database — Clerk is identity only.

## Consequences
- + Fast start, hosted invite/MFA flows, ready React components, free up to 10k MAU
- − Vendor dependency; user data partly external; webhook sync must be idempotent
- Follow-up: provisioning flow on `organization.created`

## Alternatives considered
- ASP.NET Identity (self-hosted) — more work, no vendor lock-in
- Auth0 — Organizations limited on free tier
- Supabase Auth — no org concept
