# Data Model

> Purpose: entities and relations. Status: Draft — TODO ERD

## Base columns
All tenant entities: `id (uuid v7)`, `tenant_id`, `created_at`, `created_by`, `updated_at`, `updated_by`, `deleted_at`, `assigned_user_id?`, `custom_fields jsonb`.

## Entities by module
- **Identity**: Tenant, User, TenantMembership, Role, RolePermission, Team, TeamMember
- **Settings**: Picklist, PicklistItem, TaxRate, TenantSettings, CustomFieldDefinition (built, see "Settings tables")
- **Crm**: Company, Contact, Tag (built, see "Crm tables"), Note, Activity
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

**`crm.companies`**: `id`, `tenant_id`, `name` (200, required), `industry?` (100), `website?` (300), `phone?` (30), `email?` (254), `pan_number?` (9 digits, Nepal PAN/VAT), address columns, `owner_user_id?` (null = unassigned; the web form defaults to the creator), `team_id?`, audit columns, `deleted_at?`. Indexes: `(tenant_id, name)`, `(tenant_id, owner_user_id)`, `(tenant_id, team_id)`.

**`crm.contacts`**: `id`, `tenant_id`, `first_name` (100, required), `last_name?` (100), `job_title?` (100), `company_id?` (FK `crm.companies`, restrict; at most one company per contact), `email?`, `phone?`, `mobile?`, address columns (`email`, `phone`, `mobile` and the address are `ContactPii`), `owner_user_id?` (null = unassigned), `team_id?`, audit columns, `deleted_at?`. Indexes: `(tenant_id, company_id)`, `(tenant_id, owner_user_id)`, `(tenant_id, team_id)`, plus the EF default `(company_id)` for the FK.

Deleting a company is a soft delete; its contacts keep `company_id` and the API shows no company name for them. The audit log lists the address as flattened members (`address.city`, ...); `contacts.address` is `ContactPii`, so it appears as `{ "redacted": true }`.

**`custom_fields jsonb`** (not null, default `{}`) on `crm.companies` and `crm.contacts`: values keyed by the field key of an active `CustomFieldDefinition` (text and select as strings, number as a JSON number, date as `yyyy-MM-dd` AD, checkbox as boolean). Values of deleted definitions stay in the column but are never returned and are kept when the record is saved.

**`crm.tags`** (tenant-owned, RLS, audit columns, hard delete): `id`, `tenant_id`, `name` (50, required), `color` (text: `neutral | informative | positive | notice | negative`, design token names). Unique index `ux_tags_tenant_id_name_lower` on `(tenant_id, lower(name))` (created by SQL in the migration). One list is shared by companies and contacts; tags are created on the fly when a record is saved with an unknown name.

**`crm.company_tags`** (`company_id`, `tag_id`, `tenant_id`; PK `(company_id, tag_id)`) and **`crm.contact_tags`** (`contact_id`, `tag_id`, `tenant_id`; PK `(contact_id, tag_id)`): tenant-owned (RLS), FKs to the record and to `crm.tags` with cascade delete, index `(tenant_id, tag_id)`. Assignment rows are audited as Created or Deleted under the entity type `crm.company_tags` / `crm.contact_tags` and the record id as entity id (the changes hold the `tagId`).

## Settings tables (schema `settings`)
**`settings.custom_field_definitions`** (tenant-owned, RLS, audit columns, `deleted_at` soft delete): `id`, `tenant_id`, `entity_type` (text: `company | contact`), `key` (slug `[a-z][a-z0-9_]{0,39}`, immutable), `label` (80), `type` (text: `text | number | date | select | checkbox`, immutable), `required`, `options text[]` (select only, 1 to 50 values of at most 80 characters), `sort_order`, `show_in_list`. Unique `(tenant_id, entity_type, key)` where `deleted_at is null`. At most 50 active definitions per entity type.

