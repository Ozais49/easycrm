# UI Design Guide

> Purpose: how CEM looks and behaves. Status: Draft — **light + dark mode confirmed**; foundations follow **[Adobe Spectrum](https://spectrum.adobe.com/)** ([ADR-0012](../adr/0012-adobe-spectrum-design-foundations.md))

## Design system: Adobe Spectrum (guidelines only)
- Spectrum is the reference for **foundations**: color, typography, spacing, sizing, iconography, states, motion, accessibility, writing.
- Implementation stays **shadcn/ui + Tailwind** ([ADR-0007](../adr/0007-shadcn-tailwind-ui.md)): Spectrum values become our CSS tokens and shadcn components are styled to match. No React Spectrum / Spectrum CSS / React Aria packages.
- Where this guide and Spectrum disagree, this guide wins (e.g. tenant branding, mobile layout); note the deviation here.
- **Platform scale**: Spectrum "medium" sizing on desktop, "large" on touch/mobile.
- **Color**: Spectrum's gray scale + semantic colors (accent, informative, negative, notice, positive) for light and dark; tenant brand overrides only the accent (`--primary`).
- **Typography**: Spectrum type scale. Font: **Source Sans 3** (Adobe's open-source sans, SIL Open Font License, free for commercial use; closest open match to Adobe Clean), with **Source Code Pro** (OFL) for monospace. Adobe Clean is proprietary and not used.
- **Writing**: Spectrum voice and tone, together with [content-voice](content-voice.md).

## Principles
1. **Content first** — portfolio media and client data are the hero; chrome stays quiet.
2. **Mobile is first-class** — every screen works at 360px; no desktop-only features.
3. **Show only what you may use** — modules, actions and fields the user lacks permission for are hidden, not disabled.
4. **One primary action per screen.**
5. **Fast to scan** — clear hierarchy, consistent record layouts.

## Breakpoints
| Name | Min width | Layout |
|---|---|---|
| base | 0 | Single column, bottom tab bar, cards instead of tables |
| md | 768px | Collapsible sidebar, tables allowed |
| lg | 1024px | Persistent sidebar, detail + side panel |
| xl | 1280px | Wider content, multi-column dashboards |

## App shell
- Desktop: left sidebar (modules by permission), top bar (org switcher, theme toggle, user menu today; global search, quick create and notifications are added when they exist)
- Mobile: top bar + bottom tabs, only for features that exist (today Home and Settings; Calendar, Inbox, Portfolio join as they ship)

## Layout patterns
- **List view**: search, filters, saved views, sortable table (md+) / card list (base), bulk actions, empty state
- **Detail view**: header (title, status, primary action), field panels, related panels; right-side activity stream on lg+, tab on mobile
- **Kanban**: Deals by stage, Deliverables by status; horizontal scroll on mobile
- **Calendar**: month / week / day / agenda; agenda default on mobile; AD + BS dates
- **Gallery**: masonry/grid, lightbox, presenter (full-screen) mode for showing clients
- **Forms**: single column on mobile, two columns on lg+; inline validation; sticky save bar

## Design tokens
- Color: `--background`, `--foreground`, `--primary`, `--muted`, `--accent`, `--destructive`, `--border`, status colors — values from Spectrum's color system, defined in `web/src/styles/tokens.css`. **Approximations:** the gray scale and the blue / green / orange / red steps are from memory of Spectrum's published hex values, not an export; the accent and status steps were nudged for WCAG AA contrast (e.g. blue-800 `#0d66d0` as light primary, green-1000 for positive). Verify against spectrum.adobe.com before relying on exact values
- Every token defined for **light and dark** themes; theme = system preference with user toggle (persisted)
- Never hard-code colors — brand change must be a token-only change
- Tenant branding overrides `--primary` (+ logo)
- Typography: Spectrum type scale; `--font-sans: "Source Sans 3", system-ui, sans-serif`, `--font-mono: "Source Code Pro", ui-monospace, monospace`
- Fonts are **self-hosted** from Cloudflare Pages as variable WOFF2, Latin subset only, `font-display: swap`. No Google Fonts CDN, no third-party requests.
- Spacing, sizing and radius: Spectrum values (4px-based scale)

## Performance
- Small bundles: route-level code splitting, lazy-load heavy views (calendar, kanban, gallery)
- Images from R2/CDN in responsive sizes; lazy loading; never full-size originals in lists

## Components inventory (planned)
DataTable, RecordCard, RecordDetail, StreamPanel, Kanban, FilterBar, BsDatePicker, MoneyField (permission-aware), MediaUploader, Gallery, Lightbox, EmptyState, ConfirmDialog, PermissionGate

## Iconography
- **lucide-react** (decided). 16px in controls, 20px in the bottom tabs; decorative icons carry no label, icon-only buttons need `aria-label`.

## Accessibility
- WCAG 2.2 AA contrast, keyboard navigable, visible focus, labels on all inputs, touch targets ≥ 44px

## TODO
- Mood board / references, logo, motion guidelines
