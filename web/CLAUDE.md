# Frontend — Claude Instructions

> Status: Draft. Read with [root CLAUDE.md](../CLAUDE.md), [ui-design-guide](../docs/guides/ui-design-guide.md) and [style-guide-frontend](../docs/guides/style-guide-frontend.md).

## Stack
React + Vite + TypeScript, shadcn/ui + Tailwind v4, TanStack Router + Query, react-hook-form + zod, Clerk React SDK. Responsive web only — **no PWA, no native app**.

## Folder structure
```
web/src/
  app/            # router, providers, layout shell
  components/ui/  # shadcn primitives (owned, editable)
  components/     # shared composite components (DataTable, RecordDetail, StreamPanel, Kanban, BsDatePicker)
  features/<module>/  # pages, hooks, forms, api per module
  lib/            # api client (orval-generated + fetch mutator), permissions, date (AD/BS), formatting
  styles/         # tokens, Tailwind theme
```

## Rules
- Server state only via TanStack Query; no ad-hoc fetch in components.
- Navigation & actions gated by the `/api/me` permission map (`usePermission(scope, action)`), but never rely on it for security.
- Sensitive fields may be **absent** from API responses — components must render gracefully (hide, not show "undefined").
- Every page must work at 360px width. Tables collapse to cards on small screens.
- Dates: show AD + BS per user/tenant preference via `lib/date`. Never format dates inline.
- Use design tokens; no hard-coded colors. Every screen must work in **light and dark** mode. Brand color is a placeholder — changing it must only touch tokens.
- Keep bundles small: lazy-load heavy views (calendar, kanban, gallery); show CDN image variants, never originals.
- Client companies are labeled "Company" (placeholder name).

## Commands
```bash
cd web
npm install
cp .env.example .env.local   # then fill in the variables below
npm run dev          # http://localhost:5173 (runs api:generate first)
npm run build        # tsc -b && vite build (runs api:generate first)
npm run lint         # ESLint (flat config, typescript-eslint, react-hooks)
npm run format       # Prettier
npm run api:generate # regenerate src/lib/api/generated from openapi/cem.json
```

## Environment (`web/.env.local`, see `.env.example`)
| Variable | Meaning |
|---|---|
| `VITE_API_BASE_URL` | API origin, e.g. `http://localhost:5080` |
| `VITE_CLERK_PUBLISHABLE_KEY` | Clerk publishable key. Without it the app shows a setup message instead of crashing |

## API client (orval)
- `openapi/cem.json` is a committed snapshot of the backend spec. `src/lib/api/generated/` (TanStack Query hooks + types) is generated from it by **orval** and gitignored; `predev`/`prebuild` regenerate it.
- Refresh the snapshot after changing API endpoints: `dotnet run --project src/Cem.Host` (Development), then `curl http://localhost:5080/openapi/v1.json -o web/openapi/cem.json`, then `npm run api:generate`. Commit the snapshot.
- Every backend endpoint needs `.WithName("PascalCase")` and typed `.Produces<T>()` / `.ProducesProblem()`, or orval generates `unknown`.
- `src/lib/api/client.ts` is the orval mutator: base URL, Clerk token (getter registered once in `AuthGate`), RFC 7807 problems parsed into `ApiError`.
- Switching organization remounts the app with a fresh QueryClient (`OrgApp` is keyed by org id), which clears the cache.
