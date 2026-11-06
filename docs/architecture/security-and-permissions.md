# Security & Permissions

> Purpose: technical design for RBAC and sensitive data. Status: Draft. Product view: [roles-and-permissions](../product/roles-and-permissions.md)

## Authentication
Clerk — [ADR-0003](../adr/0003-clerk-for-authentication.md). The API validates session JWTs via the Clerk JWKS (JwtBearer); Clerk webhooks (svix-signed) sync users, orgs and memberships into our DB ([ADR-0015](../adr/0015-tenant-resolution-and-provisioning.md)).
- Config section `Clerk`: `Authority` (Frontend API URL; issuer and JWKS discovery), `AuthorizedParties` (allowed `azp` origins), `WebhookSigningSecret` (`whsec_...`). Real values go in `dotnet user-secrets` / environment, never in git. Outside Development missing values stop startup; in Development the app starts without them and authenticated calls return 401.
- Validated: issuer, signature, lifetime, and `azp` (rejected when present and not in `AuthorizedParties`). Audience is not validated: Clerk session tokens carry no `aud` by default.
- Every endpoint requires an authenticated user (fallback policy); `/health`, `/openapi` and `/webhooks/clerk` are anonymous.
- `/webhooks/clerk`: verifies the Svix signature (`HMAC-SHA256` over `{svix-id}.{svix-timestamp}.{raw body}` with the base64 secret, constant-time compare, any `v1` entry may match, timestamp within 5 minutes). Bad signature or payload → 400.

## Authorization
- `IPermissionService.Can(scope, action, record?)` evaluates level `all | team | own | none` against record owner/team.
- List queries apply the level as a filter (own → `assigned_user_id = me`; team → user's teams).
- Endpoints declare their required permission; a Mediator pipeline behavior enforces it.
- `/api/me` returns the effective permission map for UI gating (cosmetic only).

## Field-level security
- DTO properties tagged `[Sensitive(FieldGroup.Pricing | Financial | ContactPii)]`.
- A System.Text.Json `JsonTypeInfo` modifier omits properties the caller cannot read.
- Writes to sensitive properties without edit permission → TODO: reject (403) vs silently ignore.
- Exports, PDFs, search results, dashboards and notifications must apply the same rules.

## Audit
- Audit log for create/update/delete of business entities; always for sensitive-field, role and permission changes.

## Secrets & webhooks
- Integration tokens encrypted at rest (ASP.NET Data Protection; key ring persisted in DB).
- Signatures validated: Meta `X-Hub-Signature-256`, Clerk (svix, implemented), TikTok.

## Public Content API keys
- Per-tenant API keys created in Settings; stored hashed (show once), scoped (e.g. `portfolio:read`), revocable
- Public API returns only published items and watermarked media URLs; never sensitive fields

## TODO
- Rate limiting (per user + per API key), CORS, CSP, upload size/type limits and scanning
