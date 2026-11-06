# Modules

> Purpose: module boundaries, contracts and events. Status: Draft

| Module | Owns | Publishes (events) | Consumes |
|---|---|---|---|
| Identity/Tenancy | Tenant, User, Membership, Role, Team | TenantCreated, UserJoined | Clerk webhooks |
| Settings | Picklists, tax rates, branding, custom field definitions | SettingsChanged | — |
| Crm | Company, Contact, Note, Activity, Tag | ContactCreated | LeadConverted |
| Calendar | Event (+ `is_public`), Attendee, Reminder, WorkingHours, Availability (computed) | — | DeliverableDueDateChanged |
| Sales | Lead, Deal, Product, Quote, Invoice | DealWon, LeadConverted | ContactCreated, LeadCaptured |
| Projects | Project, Deliverable, Task, Attachment | DeliverableDueDateChanged, DeliverableApproved | DealWon |
| Portfolio | PortfolioItem, MediaAsset, Collection, ShareLink, WatermarkSettings, Update (announcements) | ContentPublished | DeliverableApproved (optional promote to portfolio) |
| PublicApi (host-level) | ApiKey | — | Reads Portfolio/Calendar contracts; serves `/public/v1` |
| Inbox | Channel, IntegrationAccount, Conversation, Message | MessageReceived, LeadCaptured | — |

## Rules
- Other modules access data only via `<Module>.Contracts` (query interfaces + event types) or integration events.
- Events go through the outbox — [ADR-0005](../adr/0005-integration-events-outbox.md).
- One Postgres schema per module (e.g. `crm.contacts`) — confirmed, see [ADR-0014](../adr/0014-backend-conventions.md).
