# CEM

Multi-tenant CRM. .NET 10 API (`src/`) + React web (`web/`). Project overview, stack and all docs: [docs/README.md](docs/README.md)

## Commands
```bash
dotnet tool restore                      # once: installs dotnet-ef
dotnet build                             # warnings are errors
dotnet run --project src/Cem.Host        # http://localhost:5080  (/health, /openapi/v1.json in Development)
# new migration for a module context (replace placeholders):
dotnet ef migrations add <Name> --project src/Modules/<Module>/<Module>.Infrastructure --startup-project src/Cem.Host --context <Module>DbContext
dotnet ef database update --project src/Modules/<Module>/<Module>.Infrastructure --startup-project src/Cem.Host --context <Module>DbContext
```
Deploy: TODO.

## Rules
- Tenant isolation: every tenant entity implements `ITenantEntity`; never `IgnoreQueryFilters` outside platform admin.
- Permissions are enforced server-side; `[Sensitive]` fields are stripped by the serializer. UI gating is cosmetic.
- Modules talk only via `*.Contracts` or outbox events.
- Runs on a 1 GB VPS shared with Postgres: no heavy libraries; images/PDFs go to background jobs; files go browser ↔ R2 directly.
- No automated tests for now; don't create test projects.
- One feature at a time, then stop for manual verification.
- Significant decision → new ADR in `docs/adr/`. Behaviour change → update the related doc in the same PR.

## Git
- Feature branches + PRs; never commit to `main`. Conventional commits.
- Never mention Claude/AI in commits or PRs (no trailers).

## Read before working on
- Permissions / tenancy → `docs/architecture/security-and-permissions.md`, `docs/architecture/multi-tenancy.md`
- UI → `docs/guides/ui-design-guide.md`
- Public API / portfolio media → `docs/adr/0011-public-content-api-and-watermarking.md`
- Dates, money, VAT → `docs/guides/localization-guide.md`
- Open decisions → `docs/product/open-questions.md`



Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.