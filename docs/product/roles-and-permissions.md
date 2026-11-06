# Roles & Permissions

> Purpose: who can see/do what. Status: Draft. Technical design: [security-and-permissions](../architecture/security-and-permissions.md)

## Model
- **Scope permissions** per entity: `create / read / edit / delete`, each with level `all | team | own | none`.
- **Field groups** (separate read/edit permission):
  - `pricing` — product prices, deal amounts, quote line prices
  - `financial` — invoice totals, payments, project budget, revenue widgets
  - `contactPii` — phone, email, address of Contacts/Leads
- A user's effective permissions = union of their roles. All roles editable by tenant Admin.

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

TODO: validate matrix with the org.
