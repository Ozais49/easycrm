# Deployment

> Purpose: environments and hosting. Status: Draft. Decision: [ADR-0009](../adr/0009-cloudflare-plus-low-end-vps.md)

## Topology
| Component | Host | Notes |
|---|---|---|
| Web (SPA) | Cloudflare Pages | Static build |
| Media & static files | Cloudflare R2 + CDN | Presigned direct uploads; watermarked variants; PDFs |
| API | Low-end Linux VPS | .NET 10 behind reverse proxy (Caddy assumed — TODO); memory budget per ADR-0009 |
| Postgres | **Same VPS** (1 GB RAM / 1 vCPU total) | Tuned for low memory per ADR-0009; nightly pg_dump → R2; RLS required |
| Auth | Clerk free | |

## Release flow
Feature done → manual verification → deploy → status/feedback recorded ([ADR-0010](../adr/0010-no-automated-tests-initially.md)).

## Environments
- local, production; staging TODO

## TODO
- OS hardening, swap file, systemd vs Docker, CI/CD via GitHub Actions (build + deploy only), secrets, backups (pg_dump → R2), monitoring, TLS/domain, migrations on deploy
