# Frontend Style Guide (TypeScript / React)

> Purpose: consistent frontend code. Status: Draft

## General
- TypeScript strict; no `any` (use `unknown` + narrowing)
- ESLint + Prettier — TODO configure
- Function components only; named exports

## Naming
- Components `PascalCase.tsx`; hooks `useThing.ts`; utils `camelCase.ts`
- Feature folders: `features/<module>/{pages,components,hooks,api}`

## Data
- TanStack Query for all server state; query keys factory per module (`contactKeys.detail(id)`)
- API client generated from OpenAPI with **orval** (TanStack Query hooks + zod); never hand-write request types
- Optimistic updates only where rollback is trivial (kanban moves)

## Forms
- react-hook-form + zod; schema next to the form; server validation errors mapped to fields

## Permissions
- `usePermission(scope, action)` and `<PermissionGate>`; treat missing sensitive fields as "not visible"

## Styling
- Tailwind utilities with design tokens only; no inline hex colors
- Compose shadcn primitives; don't fork them unless needed (document if forked)
- Style components to match [Adobe Spectrum](https://spectrum.adobe.com/) foundations ([ADR-0012](../adr/0012-adobe-spectrum-design-foundations.md)); don't add Spectrum/React Aria packages

## Dates & money
- Always via `lib/date` (AD/BS) and `lib/money` helpers

## TODO
- State for UI-only concerns (Zustand?), routing conventions, error boundary pattern
