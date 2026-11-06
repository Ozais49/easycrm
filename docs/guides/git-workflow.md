# Git Workflow

> Purpose: branching, commits, PRs. Status: Draft

## Branches
- `main` is protected — never commit directly
- Feature branches: `feat/<scope>-<short>`, `fix/...`, `chore/...`, `docs/...`, `refactor/...`

## Commits
- Conventional Commits: `type(scope): summary` — e.g. `feat(sales): add deal kanban`
- Types: feat, fix, docs, refactor, test, chore, perf, build, ci
- Scope = module (`crm`, `sales`, `portfolio`, `inbox`, `web`, `infra`, ...)
- **No mention of Claude/AI** — no `Co-Authored-By` trailers, no "Generated with" lines, in commits or PRs

## Pull requests
- Small and focused; link the issue/ADR
- Description: what, why, how tested, screenshots for UI (mobile + desktop)
- Must pass CI; update docs/ADRs in the same PR when behaviour or decisions change

## TODO
- Remote host (GitHub assumed), PR template file, required reviewers, merge strategy (squash assumed), release tagging
