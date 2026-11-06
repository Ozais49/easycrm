# Product Requirements

> Purpose: what CEM does, per module. Status: Draft

## Target users
Small creative & service-based orgs (studios, agencies, photographers, consultants). Later: many orgs as SaaS tenants.

## Modules
### Core CRM
- Companies (client organizations; placeholder label), Contacts, Tags, Teams
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
- Tenant branding, timezone, currency, tax rates, picklists (stages, statuses, event types)
- Users & invites, Teams, Roles (permission matrix), Integrations, Custom fields

### Dashboard
- Today's events, my deliverables, new messages, pipeline value (permission-gated)

## Non-functional
- Responsive: usable at 360px width through desktop
- Multi-tenant isolation (see [multi-tenancy](../architecture/multi-tenancy.md))
- TODO: performance, backup, data retention targets
