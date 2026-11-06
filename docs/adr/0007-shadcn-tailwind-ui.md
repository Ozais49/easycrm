# ADR-0007: shadcn/ui + Tailwind v4 for the UI

- Status: Accepted
- Date: 2026-11-06

## Context
Need a responsive, themeable UI with per-tenant branding and a fresh look (EspoCRM only as loose feature inspiration).

## Decision
React + Vite + TypeScript with shadcn/ui components (copied into the repo, owned by us) and Tailwind v4 design tokens.

## Consequences
- + Full control over look; lightweight; good accessibility via Radix primitives
- − We maintain the components; complex data grids need extra work (TanStack Table)

## Alternatives considered
- Ant Design — heavy, corporate look
- MUI — Material look, harder to rebrand
