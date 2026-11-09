# Product Requirements

> Purpose: what CEM does, per module. Status: Draft

## Target users
Small creative & service-based orgs (studios, agencies, photographers, consultants). Later: many orgs as SaaS tenants.

## Modules
### Core CRM
- **Built (2.1):** Companies (client organizations; placeholder label) and Contacts: list with name search, detail, create, edit, soft delete. Each record has an owner and an optional team; access follows the `crm` permission level (all, team or own). A contact belongs to at most one company (the same person at two companies is two unlinked contact records). Contact email, phone, mobile and address are `ContactPii`. Companies carry a Nepal PAN/VAT number (9 digits) and a province-based address
- **Deferred:** Tags, custom fields on records, activity stream, global search
- Teams (see Settings)
- Activity stream per record (notes, calls, meetings, messages), global search
- Custom fields per entity (admin-defined, JSONB)

### Calendar
- Events: meeting, shoot, call, deadline; attendees; reminders
- Month / week / day / agenda views; deliverable due dates appear as events
- AD + Bikram Sambat display
- Sync v1: read-only **ICS feeds** (per user + public events); Google two-way sync deferred

### Sales
- Leads → convert to Contact/Company/Deal
- Deals with configurable pipeline stages (kanban)
- Products/Services catalog, Quotes (PDF), Invoice-lite with payment status
- Configurable tax/VAT
- Prices & totals gated by `field:pricing` / `field:financial`

### Projects & Deliverables
- Projects linked to Company/Deal; Deliverables with status, assignee, due date, revisions; Tasks; attachments
- Kanban by status
- Workflow: a deliverable moves up status by status as work is done → **manual internal verification** → **delivered** to the client → client **returns status/feedback** (approved / changes requested → back to in-progress, revision count +1)
- Statuses (fixed): To do → In progress → Internal review → Delivered → Approved | Changes requested

### Portfolio
- Portfolio items (title, client, category, tags, description, visibility) with images/videos
- Gallery + lightbox, curated collections, show to clients directly from the CRM
- **Watermarked** display variants for all shared/public media; originals stay private ([ADR-0011](../adr/0011-public-content-api-and-watermarking.md))
- Tokenized share links for collections — **expiring and password-protected**
- Watermark configurable per tenant (logo/text, position, opacity, size)

### Updates
- Short posts/announcements (news, new work, offers) with optional media; draft/published

### Public Content API
- Read-only REST API so other apps can link to CEM: **portfolio, public calendar events, available time slots, updates** ([ADR-0011](../adr/0011-public-content-api-and-watermarking.md))
- Per-tenant API keys managed in Settings; public ICS feed for public events
- Availability = working hours minus busy events (no private details exposed); **read-only in v1** — no booking

### Inbox
- Unified inbox: WhatsApp Business, Instagram DMs; TikTok lead capture
- Link conversations to Contacts/Leads, assign, reply

### Settings / Admin
- **Built (Phase 1):** tenant settings — time zone, date display (AD/BS/both), brand color (overrides `--primary`); created for each new tenant from `TenantCreated`, readable by every member, edited with `Settings:Edit`
- **Deferred:** logo (needs R2 uploads); picklists (stages, statuses, event types), tax rates, watermark settings and custom fields arrive with the modules that use them (Sales, Calendar, Portfolio)
- Currency is fixed to NPR (see localization guide)
- Users & invites, Teams, Roles (permission matrix), Integrations, Custom fields

### Dashboard
- Today's events, my deliverables, new messages, pipeline value (permission-gated)

## Non-functional
- Responsive: usable at 360px width through desktop
- Multi-tenant isolation (see [multi-tenancy](../architecture/multi-tenancy.md))
- TODO: performance, backup, data retention targets
