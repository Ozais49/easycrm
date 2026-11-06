# Frontend — Claude Instructions

> Status: Draft. Read with [root CLAUDE.md](../CLAUDE.md), [ui-design-guide](../docs/guides/ui-design-guide.md) and [style-guide-frontend](../docs/guides/style-guide-frontend.md).

## Stack
React + Vite + TypeScript, shadcn/ui + Tailwind v4, TanStack Router + Query, react-hook-form + zod, Clerk React SDK. Responsive web only — **no PWA, no native app**.

## Folder structure (planned)
```
web/src/
  app/            # router, providers, layout shell
  components/ui/  # shadcn primitives (owned, editable)
  components/     # shared composite components (DataTable, RecordDetail, StreamPanel, Kanban, BsDatePicker)
  features/<module>/  # pages, hooks, forms, api per module
  lib/            # api client (generated from OpenAPI — TODO tool), permissions, date (AD/BS), formatting
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
TODO
