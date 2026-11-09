# Data Model

> Purpose: entities and relations. Status: Draft — TODO ERD

## Base columns
All tenant entities: `id (uuid v7)`, `tenant_id`, `created_at`, `created_by`, `updated_at`, `updated_by`, `deleted_at`, `assigned_user_id?`, `custom_fields jsonb`.

## Entities by module
- **Identity**: Tenant, User, TenantMembership, Role, RolePermission, Team, TeamMember
- **Settings**: Picklist, PicklistItem, TaxRate, CustomFieldDefinition, TenantSettings
- **Crm**: Company, Contact (built, see "Crm tables"), Tag, Note, Activity
- **Calendar**: Event (+ `is_public`), EventAttendee, Reminder, WorkingHours
- **Sales**: Lead, Deal, PipelineStage, Product, Quote, QuoteLine, Invoice, InvoiceLine, Payment
- **Projects**: Project, Deliverable, DeliverableRevision, Task, Attachment
- **Portfolio**: PortfolioItem (+ `is_published`), MediaAsset (original key, watermarked variant keys, thumbnail keys), Collection, CollectionItem, ShareLink (expires_at, password_hash), WatermarkSettings, Update
- **Settings (public API)**: ApiKey (hashed, scopes, revoked_at)
- **Inbox**: IntegrationAccount, Conversation, Message, ConversationParticipant
- **Platform** (schema `platform`): AuditEntry, OutboxMessage (below), FileObject (planned)

## TODO
- ERD, cardinalities, indexes
- Money: `numeric(18,2)`, always NPR (single currency, no per-row currency column)

## Platform tables
**`platform.audit_entries`** (tenant-owned, RLS): `id`, `tenant_id`, `occurred_at`, `user_id?` (null = system), `action` (`Created` | `Updated` | `Deleted`, soft delete = Deleted), `entity_type` (`schema.table`, e.g. `identity.roles`), `entity_id`, `changes jsonb` (`{ "property": { "old", "new" } }`; `{ "redacted": true }` for `[Sensitive]` properties). Indexes: `(tenant_id, entity_type, entity_id, occurred_at desc)`, `(tenant_id, occurred_at desc)`.

**`platform.outbox_messages`** (not tenant-scoped, no RLS): `id`, `tenant_id?`, `event_type`, `payload jsonb`, `occurred_at`, `processed_at?`, `attempts`, `last_error?`, `next_attempt_at`, `dead_at?`. Partial index on `next_attempt_at` where `processed_at` and `dead_at` are null.

## Crm tables (schema `crm`)
Both tables are tenant-owned (RLS, `deleted_at` soft delete, audit columns). The address is stored as four columns of the owning row (`address_street`, `address_city`, `address_district`, `address_province`; province is text: `koshi | madhesh | bagmati | gandaki | lumbini | karnali | sudurpashchim`). `owner_user_id` and `team_id` have no foreign key: they point into the Identity module, which Crm reaches only through `Cem.Identity.Contracts` (`IMemberDirectory` validates them on write).

**`crm.companies`**: `id`, `tenant_id`, `name` (200, required), `industry?` (100), `website?` (300), `phone?` (30), `email?` (254), `pan_number?` (9 digits, Nepal PAN/VAT), address columns, `owner_user_id` (required, defaults to the creator), `team_id?`, audit columns, `deleted_at?`. Indexes: `(tenant_id, name)`, `(tenant_id, owner_user_id)`, `(tenant_id, team_id)`.

**`crm.contacts`**: `id`, `tenant_id`, `first_name` (100, required), `last_name?` (100), `job_title?` (100), `company_id?` (FK `crm.companies`, restrict; at most one company per contact), `email?`, `phone?`, `mobile?`, address columns (`email`, `phone`, `mobile` and the address are `ContactPii`), `owner_user_id`, `team_id?`, audit columns, `deleted_at?`. Indexes: `(tenant_id, company_id)`, `(tenant_id, owner_user_id)`, `(tenant_id, team_id)`, plus the EF default `(company_id)` for the FK.

Deleting a company is a soft delete; its contacts keep `company_id` and the API shows no company name for them. The audit log records email, phone and mobile as redacted; address columns are not scalar entity properties, so address changes are not listed in the audit entry.
