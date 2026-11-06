# ADR-0011: Watermarked media and a public Content API

- Status: Accepted
- Date: 2026-11-06

## Context
Portfolio work is shown to clients directly from the CRM and must be protected from reuse. Other applications (the org's website, booking pages, partner apps) need to link to CEM and pull published content: portfolio, calendar, updates and available time slots.

## Decision
### Watermarking
- On upload, a background job creates watermarked display variants + thumbnails, stored in R2. Originals stay private.
- Only watermarked variants are served on share links and the public API.
- Watermark is **configurable per tenant**: logo or text, position, opacity, size.

### Share links
- Tokenized links to collections/items, **expiring** and **password-protected** (password hashed; expiry required with a tenant default).

### Public Content API (REST, `/public/v1`)
Read-only, per-tenant API keys (hashed, scoped, revocable), rate-limited, edge-cached on Cloudflare.

| Resource | Scope | Content |
|---|---|---|
| `/public/v1/portfolio` | `portfolio:read` | Published items, collections, watermarked media URLs |
| `/public/v1/calendar/events` | `calendar:read` | Only events marked **public** (title, time, location) — never private details |
| `/public/v1/availability` | `availability:read` | Free time slots computed from working hours minus busy events (no event details) |
| `/public/v1/updates` | `updates:read` | Published updates/announcements (news, new work, offers) |

- Also exposed as an **ICS feed** for public events (calendar apps subscribe)
- TODO: outbound webhooks to notify linked apps on publish (v2)
- TODO: booking/requesting a slot from outside (open question #7d) — read-only in v1

## Consequences
- + Other apps link to CEM without duplicate data entry; CDN absorbs traffic
- − Extra storage for variants; image processing must stay memory-light (NetVips, concurrency 1 — ADR-0009)
- − Availability must be computed cheaply (cached per day, invalidated on event change)
- Follow-up: cache purge on publish/unpublish, working-hours settings per user/tenant

## Alternatives considered
- On-the-fly watermarking per request — too heavy for the VPS
- Cloudflare Images transformations — possible later; paid
