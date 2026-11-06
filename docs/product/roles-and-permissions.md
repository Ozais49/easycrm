# Roles & Permissions

> Purpose: who can see/do what. Status: Draft. Technical design: [security-and-permissions](../architecture/security-and-permissions.md), [ADR-0016](../adr/0016-rbac-model.md)

## Model
- **Scope permissions** per entity: `create / read / edit / delete`, each with level `all | team | own | none`.
- **Scopes** (one per row of the matrix below): `crm` (Companies, Contacts), `calendar`, `sales` (Leads, Deals), `billing` (Quotes, Invoices), `projects` (Projects, Deliverables), `portfolio`, `inbox`, `settings` (including users, roles and teams administration).
- **Field groups** (separate read/edit permission):
  - `pricing` — product prices, deal amounts, quote line prices
  - `financial` — invoice totals, payments, project budget, revenue widgets
  - `contactPii` — phone, email, address of Contacts/Leads
- A user's effective permissions = union of their roles (highest level per scope and action; field access if any role grants it). All roles editable by tenant Admin; the Admin role itself always keeps full `settings` access.

## Seeded roles (draft)
| Scope | Admin | Manager | Sales | Creative/Staff | Viewer |
|---|---|---|---|---|---|
| Companies/Contacts | all | all | team | read own | read all |
| Calendar | all | all | own | own | read |
| Leads/Deals | all | all | team | none | none |
| Quotes/Invoices | all | all | own | none | none |
| Projects/Deliverables | all | all | read | team | read |
| Portfolio | all | all | read | edit | read |
| Inbox | all | all | team | none | none |
| Settings | all | none | none | none | none |
| field:pricing | rw | rw | rw | none | none |
| field:financial | rw | r | none | none | none |
| field:contactPii | rw | rw | rw | none | none |

### How the table is read
- `all`, `team`, `own`, `none` apply that level to all four actions.
- `read own` is Read:own only; `read all` and `read` are Read:all only (other actions none).
- Portfolio `edit` is Create, Read and Edit at `all`, Delete none.
- Field groups: `rw` is read and edit, `r` is read only, `none` is neither.
- New members get Admin when their Clerk organization role is admin, otherwise Viewer.
- The seed lives in one file: `src/Modules/Identity/Cem.Identity.Domain/SeedRoles.cs`.

TODO: validate matrix with the org.
