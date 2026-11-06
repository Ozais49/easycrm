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
- **Platform**: AuditLog, OutboxMessage, FileObject

## TODO
- ERD, cardinalities, indexes
- Money: `numeric(18,2)`, always NPR (single currency, no per-row currency column)
