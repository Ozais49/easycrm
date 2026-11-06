# Frontend Style Guide (TypeScript / React)

> Purpose: consistent frontend code. Status: Draft

## General
- TypeScript strict; no `any` (use `unknown` + narrowing)
- ESLint (flat config: typescript-eslint, react-hooks; `no-explicit-any` is an error) and Prettier (no semicolons, single quotes, width 100) are configured: `npm run lint`, `npm run format`
- Function components only; named exports

## Naming
- Components `PascalCase.tsx`; hooks `useThing.ts`; utils `camelCase.ts`
- Feature folders: `features/<module>/{pages,components,hooks,api}`

## Data
- TanStack Query for all server state; query keys factory per module (`contactKeys.detail(id)`)
- API client generated from the OpenAPI snapshot (`web/openapi/cem.json`) with **orval** (TanStack Query hooks + types, `npm run api:generate`); never hand-write request or response types. Forms validate with zod schemas written next to the form
- Errors: the mutator (`lib/api/client.ts`) parses RFC 7807 problem responses into `ApiError` (`status`, `code` from the `code` extension, `title`, `fieldErrors` from `errors`). `friendlyMessage(error)` turns it into "what happened + what to do" text and never shows codes; `applyFieldErrors` copies `fieldErrors` onto react-hook-form fields (case-insensitive), falling back to a toast
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
