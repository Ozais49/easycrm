# Open Questions

> Status: Living document. Resolve → record answer here and, if architectural, write an ADR.

| # | Question | Answer / assumption | Status |
|---|---|---|---|
| 1 | Base currency; single vs multi-currency? | **NPR, single currency** | RESOLVED |
| 1a | Number grouping | **Lakh/crore** (Rs. 1,00,000) | RESOLVED |
| 2 | Visual direction | **Dark mode: yes** (light + dark). **Adobe Spectrum** as design foundations, implemented with shadcn + Tailwind — [ADR-0012](../adr/0012-adobe-spectrum-design-foundations.md) | RESOLVED |
| 3 | Which tests are mandatory? | **No automated tests for now** — [ADR-0010](../adr/0010-no-automated-tests-initially.md) | RESOLVED |
| 4 | Hosting | **Cloudflare** (Pages + R2) for UI/static/media; **low-end Linux VPS** for API — [ADR-0009](../adr/0009-cloudflare-plus-low-end-vps.md) | RESOLVED |
| 4a | Postgres location | **Same VPS** as the API | RESOLVED |
| 4b | VPS spec | **1 GB RAM, 1 vCPU** (baseline for budgets) | RESOLVED |
| 5 | API style | **REST + OpenAPI** | RESOLVED |
| 6 | Deliverable workflow | **To do → In progress → Internal review → Delivered → Approved / Changes requested** (fixed; changes requested → back to In progress) | RESOLVED |
| 7 | Portfolio sharing | Watermarked media + public Content API — [ADR-0011](../adr/0011-public-content-api-and-watermarking.md) | RESOLVED |
| 7a | Share link expiry / password? | **Yes — expiring and password-protected** | RESOLVED |
| 7b | Watermark content/position/opacity | **Configurable per tenant** (logo or text, position, opacity) | RESOLVED |
| 7c | Public Content API scope | **Portfolio, calendar (public events), updates/announcements, available time slots** — integrations link other apps to ours | RESOLVED |
| 7d | Available time slots: read-only, or can outsiders also request/book a slot? | **Read-only in v1** (no booking) | RESOLVED |
| 8 | Client company label | Placeholder **"Company"** | RESOLVED (placeholder) |
| 9 | Git | **GitHub, `main`** | RESOLVED |
| 10 | Is `contactPii` field group needed? | **Yes** | RESOLVED |
| 11 | Calendar sync: ICS feed only or Google two-way? | **ICS feed only in v1**; Google sync later | RESOLVED |
| 12 | App email provider (reminders, notifications) | **Resend** — [ADR-0013](../adr/0013-resend-for-transactional-email.md) | RESOLVED |
| 13 | Frontend API client generation tool | **orval** | RESOLVED |
