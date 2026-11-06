# Data Model

> Purpose: entities and relations. Status: Draft — TODO ERD

## Base columns
All tenant entities: `id (uuid v7)`, `tenant_id`, `created_at`, `created_by`, `updated_at`, `updated_by`, `deleted_at`, `assigned_user_id?`, `custom_fields jsonb`.

## Entities by module
- **Identity**: Tenant, User, TenantMembership, Role, RolePermission, Team, TeamMember
- **Settings**: Picklist, PicklistItem, TaxRate, CustomFieldDefinition, TenantSettings
- **Crm**: Company, Contact, Tag, Note, Activity
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
