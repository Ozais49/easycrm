# ADR-0009: Cloudflare for UI/static/media, low-end Linux VPS for the API

- Status: Accepted
- Date: 2026-11-06

## Context
Hosting must be near-free. The frontend and all static content can live on a CDN; the .NET API will run on a small, low-end Linux VPS, so memory and CPU are scarce.

## Decision
- **Cloudflare Pages**: React SPA
- **Cloudflare R2** (+ CDN): media, portfolio images/videos, watermarked variants, generated PDFs, other static files. Browser uploads go directly to R2 via presigned URLs — files never stream through the API.
- **Linux VPS**: .NET 10 API behind a reverse proxy (Caddy assumed — TODO), run as a systemd service or container (TODO).
- **Optimization is mandatory**: every library is chosen with memory/CPU footprint in mind; if a library is too heavy, replace it.

## Performance budget — VPS: 1 GB RAM, 1 vCPU; Postgres on the same VPS
| Process | Memory budget |
|---|---|
| OS + reverse proxy | ~200 MB |
| PostgreSQL (tuned: `shared_buffers=128MB`, `work_mem=4MB`, `max_connections=20`) | ~250–300 MB |
| .NET API (steady state RSS) | ≤ 200 MB |
| Headroom (image jobs, spikes) | ~250 MB + swap file (1–2 GB) |

- API startup < 3 s; EF connection pool capped (e.g. `Maximum Pool Size=10`)
- Use `DOTNET_GCConserveMemory` / DATAS GC, `ServerGarbageCollector=false` if needed, `InvariantGlobalization`, trimming/ReadyToRun where compatible
- No in-process heavy work on the request path: image processing (watermark, thumbnails) via a low-memory library (libvips/NetVips over ImageSharp) in a background queue with concurrency 1
- Stream large responses; paginate everything; no unbounded queries
- `AsNoTracking` + projections for reads; compiled queries on hot paths
- Cache aggressively at the edge (Cloudflare) for public content API and media

## Library selection rule
Before adding a dependency, note its memory/CPU impact in the PR. Prefer source-generated over reflection-based libraries (e.g. Mediator, System.Text.Json source gen, Mapperly). Heavy candidates to watch: ImageSharp, QuestPDF, Hangfire, AutoMapper, SignalR scale.

## Consequences
- + Very low cost; CDN offloads most traffic
- − Single VPS = single point of failure; we own OS patching, backups, TLS
- − Postgres shares the box: DB backups (pg_dump → R2, nightly) are mandatory
- Follow-up: monitoring, backup/restore runbook

## Alternatives considered
- Render/Koyeb free tiers — cold starts, limited control
- Azure App Service F1 — CPU minute quota
