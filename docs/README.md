# CEM Documentation

> Status: Draft

## What CEM is
A lite, **multi-tenant** CRM for creative & service-based orgs: calendar, portfolio (showcase of past work to clients), sales pipeline, projects & deliverables, updates, a unified social inbox (WhatsApp, Instagram, TikTok leads) and a public Content API. Responsive web (desktop + mobile browsers). EspoCRM is a loose feature inspiration only — the design is our own.

## Stack
| Area | Choice | ADR |
|---|---|---|
| Backend | .NET 10 modular monolith, Clean layers per module, `martinothamar/Mediator`, EF Core + Npgsql | [0001](adr/0001-modular-monolith.md), [0004](adr/0004-mediator-for-cqrs.md) |
| Module comms | Integration events + outbox | [0005](adr/0005-integration-events-outbox.md) |
| DB | PostgreSQL, shared schema, `TenantId` + EF filters + RLS | [0002](adr/0002-shared-schema-tenancy-with-rls.md) |
| Auth | Clerk (Organizations = tenants); RBAC + field-level permissions in our DB | [0003](adr/0003-clerk-for-authentication.md) |
| Custom fields | JSONB | [0006](adr/0006-jsonb-custom-fields.md) |
| Frontend | React + Vite + TS, shadcn/ui + Tailwind v4, TanStack Query/Router; responsive web, light + dark | [0007](adr/0007-shadcn-tailwind-ui.md), [0008](adr/0008-responsive-web-no-pwa.md) |
| Design system | Adobe Spectrum as design foundations (guidelines only; built with shadcn + Tailwind) | [0012](adr/0012-adobe-spectrum-design-foundations.md) |
| Email | Resend (HTTP API, sent from background jobs) | [0013](adr/0013-resend-for-transactional-email.md) |
| Hosting | Cloudflare Pages + R2; API + Postgres on a 1 GB / 1 vCPU Linux VPS | [0009](adr/0009-cloudflare-plus-low-end-vps.md) |
| Backend conventions | Built-in logging (no Serilog), FluentValidation, manual mapping, built-in OpenAPI, schema per module, central package management, `TimeProvider`, UUID v7 | [0014](adr/0014-backend-conventions.md) |
| Testing | No automated tests for now; manual verification per feature | [0010](adr/0010-no-automated-tests-initially.md) |
| Public content | Watermarked media, `/public/v1` Content API | [0011](adr/0011-public-content-api-and-watermarking.md) |
| Locale | English UI, AD + Bikram Sambat dates, NPR (lakh/crore), VAT | — |

## Docs index

| Area | Doc | Purpose |
|---|---|---|
| Product | [requirements](product/requirements.md) | Modules & features |
| Product | [roles-and-permissions](product/roles-and-permissions.md) | Roles, scope matrix, sensitive fields |
| Product | [roadmap](product/roadmap.md) | Delivery phases |
| Product | [open-questions](product/open-questions.md) | Pending decisions |
| Architecture | [overview](architecture/overview.md) | System shape & request pipeline |
| Architecture | [modules](architecture/modules.md) | Module boundaries, contracts, events |
| Architecture | [multi-tenancy](architecture/multi-tenancy.md) | Tenant isolation design |
| Architecture | [security-and-permissions](architecture/security-and-permissions.md) | RBAC, field-level security, audit |
| Architecture | [data-model](architecture/data-model.md) | Entities |
| Architecture | [integrations](architecture/integrations.md) | WhatsApp, Instagram, TikTok |
| Architecture | [deployment](architecture/deployment.md) | Hosting (free tiers) |
| Guides | [ui-design-guide](guides/ui-design-guide.md) | Design system & layout patterns |
| Guides | [style-guide-backend](guides/style-guide-backend.md) | C# conventions |
| Guides | [style-guide-frontend](guides/style-guide-frontend.md) | TS/React conventions |
| Guides | [api-guidelines](guides/api-guidelines.md) | REST conventions |
| Guides | [database-guidelines](guides/database-guidelines.md) | Postgres conventions |
| Guides | [testing-guide](guides/testing-guide.md) | Manual verification (no automated tests yet) |
| Guides | [git-workflow](guides/git-workflow.md) | Branching, commits, PRs |
| Guides | [localization-guide](guides/localization-guide.md) | BS calendar, timezone, VAT |
| Guides | [content-voice](guides/content-voice.md) | UI copy |
| Decisions | [adr/](adr/) | Architecture Decision Records |
