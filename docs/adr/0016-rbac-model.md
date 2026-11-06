# ADR-0016: RBAC model

- Status: Accepted
- Date: 2026-11-06
- Deciders: Project owner

## Context
Roles and permissions ([roles-and-permissions](../product/roles-and-permissions.md)) must be editable per tenant, enforced on the server, cheap to evaluate on a 1 GB VPS ([ADR-0009](0009-cloudflare-plus-low-end-vps.md)), and must also cover individual fields (pricing, financial, contact data).

## Decision
- **Matrix as JSONB on the role.** A role stores its whole permission matrix in one `permissions jsonb` column: level per scope and action, read/edit per field group. It is always read and edited as a whole and cached, never queried by single permission, so a row per permission would only add joins. Missing entries mean none; the immutable `PermissionMatrix` value object fills them in, so stored and returned matrices are complete. Enums are camelCase text.
- **Union rule.** Effective permissions are the union of the roles of the member: the highest level per scope and action (`none < own < team < all`), and field read/edit if any role grants it. There are no deny rules.
- **Snapshot per request, cached.** The tenant middleware loads the effective permissions and team ids once per request into the scoped `IPermissionService`; everything after reads it synchronously. Snapshots are cached 60 s per (tenant, user); a per-tenant version token inside the key is replaced when roles, assignments or team members change.
- **Endpoint filter, not a Mediator behavior.** `.RequirePermission(Scope, Action)` is an endpoint filter returning 403 `permission.denied`. Webhooks and background jobs send commands without a user, so a behavior on every command would need exceptions. Record-level `own` / `team` checks stay in handlers, which know the record.
- **Tenant resolution moves to middleware.** Minimal APIs bind the request body before endpoint filters run, and field-level write checks happen during binding, so the permissions must be loaded earlier. The `/api` group is marked with metadata and `TenantResolutionMiddleware` handles marked endpoints (refines [ADR-0015](0015-tenant-resolution-and-provisioning.md)).
- **Sensitive fields: omit on read, reject on write.** A System.Text.Json modifier hides `[Sensitive]` properties the caller cannot read. A request that sets one the caller cannot edit gets 403 `field.forbidden` instead of being silently ignored, which would lose the data of the caller without a signal. Both directions fail closed without loaded permissions.
- **Default role from Clerk.** A new membership gets Admin if the Clerk organization role is admin (`o.rol` = `admin`, v1 `org_role` and webhook `role` = `org:admin`), else Viewer. Only at creation, so changes by admins stick. This lets the first user of a new organization administer it immediately.
- **Seeding and protection.** New tenants are seeded with five roles; `Admin` is a system role that cannot be deleted, renamed or lose full `settings`, and the last active Admin cannot be demoted.

## Consequences
- + One read per user per minute for permissions; tenant admins fully control roles without migrations
- + A new scope or field group needs no schema change (the matrix is completed on read)
- + Impossible to forget a check on a field: serialization enforces it
- − Cache invalidation is per instance (fine for one VPS) and a change takes effect at the next request, not mid-request
- − Write rejection also inspects constructor-bound values after deserialization; sensitive request fields should be nullable so that not sent means null
- − The last-admin check is check-then-write: two concurrent demotions could both pass (accepted)
- Follow-up: audit log entries for role and permission changes (next feature)

## Alternatives considered
- Row per permission (`role_permissions`) — more joins and rows for data that is only used as a whole
- Mediator pipeline behavior for permissions — needs a user; background work has none
- Silently ignore forbidden field writes — hides data loss
- Check permissions on every request without cache — one more query set per call
