# Security & Permissions

> Purpose: technical design for RBAC and sensitive data. Status: Draft. Product view: [roles-and-permissions](../product/roles-and-permissions.md)

## Authentication
Clerk — [ADR-0003](../adr/0003-clerk-for-authentication.md). The API validates session JWTs via the Clerk JWKS (JwtBearer); Clerk webhooks (svix-signed) sync users, orgs and memberships into our DB ([ADR-0015](../adr/0015-tenant-resolution-and-provisioning.md)).
- Config section `Clerk`: `Authority` (Frontend API URL; issuer and JWKS discovery), `AuthorizedParties` (allowed `azp` origins), `WebhookSigningSecret` (`whsec_...`). Real values go in `dotnet user-secrets` / environment, never in git. Outside Development missing values stop startup; in Development the app starts without them and authenticated calls return 401.
- Validated: issuer, signature, lifetime, and `azp` (rejected when present and not in `AuthorizedParties`). Audience is not validated: Clerk session tokens carry no `aud` by default.
- Every endpoint requires an authenticated user (fallback policy); `/health`, `/openapi` and `/webhooks/clerk` are anonymous.
- `/webhooks/clerk`: verifies the Svix signature (`HMAC-SHA256` over `{svix-id}.{svix-timestamp}.{raw body}` with the base64 secret, constant-time compare, any `v1` entry may match, timestamp within 5 minutes). Bad signature or payload → 400.

## Authorization
Model and decisions: [ADR-0016](../adr/0016-rbac-model.md). Roles hold a permission matrix (JSONB): a level `all | team | own | none` per scope (`crm`, `calendar`, `sales`, `billing`, `projects`, `portfolio`, `inbox`, `settings`) and action (`create | read | edit | delete`), plus read/edit per field group.
- **Effective permissions** = union of the member roles: highest level per scope and action, OR on field read/edit.
- **`IPermissionService`** (scoped, SharedKernel) exposes `LevelFor`, `Can`, `CanReadField`, `CanEditField`, `TeamIds` and the whole `Matrix`. The tenant middleware loads it once per request; after that it is read synchronously (handlers, endpoint filters, the JSON serializer). Reading before it is loaded throws.
- **Snapshot cache**: one snapshot per (tenant, user) in the shared bounded memory cache (size 1, TTL 60 s). Each tenant has a version token that is part of every key; changing roles, role assignments, team members or deleting a team swaps the token, so the next request reloads. A removed or demoted member is affected within the TTL at most. Single instance only; a second API instance would need a shared invalidation channel.
- **Endpoints** declare their requirement with `.RequirePermission(Scope.X, PermissionAction.Y)`: an endpoint filter returning 403 `permission.denied` when the level is `none`. It is an endpoint filter, not a Mediator behavior, because webhooks and background jobs run commands without a user. The tenant middleware runs before every endpoint filter, so the permissions are loaded by then.
- **Record-level** `own` / `team`: handlers call `LevelFor(scope, action)` and apply it to the record owner or `TeamIds`; list queries turn it into a filter (own → `assigned_user_id = me`; team → the user teams).
- **`GET /api/me`** returns the effective permission map for UI gating (cosmetic only).
- **Administration** (`/api/members`, `/api/roles`, `/api/teams`) needs `settings` permission: read for GETs, edit for changes, delete for deleting roles and teams. The tenant always keeps one active Admin: the system role `Admin` cannot be deleted, renamed or lose full `settings`, and the last active Admin cannot lose it (409 `roles.last_admin`).

### Default role of a new member
Applied once, when the membership is created (JIT or webhook); later changes by admins are never overwritten. Clerk organization role `admin` (token `o.rol`), `org:admin` (v1 `org_role`, webhook membership `role`) gets `Admin`; anything else gets `Viewer`. New tenants are seeded with the five roles of [roles-and-permissions](../product/roles-and-permissions.md).

## Field-level security
- DTO properties tagged `[Sensitive(FieldGroup.Pricing | Financial | ContactPii)]`.
- A System.Text.Json type-info modifier on the minimal-API JSON options enforces it, using the permissions of the current request (`IHttpContextAccessor`).
- **Read**: a property whose group the caller cannot read is omitted from the response (absent, not null).
- **Write**: a request body that sets a sensitive property the caller cannot edit is rejected with 403 `field.forbidden` (the property setter throws, the host maps it). Rejecting beats silently ignoring: ignoring loses the data of the caller without telling them. Setter-bound properties are checked when set; constructor-bound ones (positional records) after the object is built, where any value other than the type default counts as sent, so sensitive request fields should be nullable (not sent = null).
- Both directions fail closed: with no HTTP context or no loaded permissions nothing sensitive is read or written.
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
