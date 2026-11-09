# ADR-0018: Emerald & Charcoal color palette

- Status: Accepted
- Date: 2026-11-09
- Deciders: Product owner

## Context
The first UI used approximated Adobe Spectrum grays and blue ([ADR-0012](0012-adobe-spectrum-design-foundations.md)). After first use, the product owner found contrast and visibility weak and chose a fixed two-tone brand palette.

## Decision
Color comes from the **Emerald & Charcoal** palette, replacing Spectrum's color system. Spectrum still guides typography, spacing, states and accessibility.

| | Background | Text / structure | Accent (`--primary`) |
|---|---|---|---|
| Light | `#FFFFFF` | `#18181B` (zinc-900) | `#059669` (emerald-600) |
| Dark | `#09090B` (zinc-950) | `#FAFAFA` (zinc-50) | `#34D399` (emerald-400) |

Other neutrals come from the zinc scale. Hover and selected states use an emerald tint (`--accent`), and status colors use Tailwind steps. All values live in `web/src/styles/tokens.css`. A tenant brand color still overrides `--primary` only.

## Consequences
- + Clearer contrast, and the brand is recognisable
- − White text on emerald-600 is about 3.8:1, which meets WCAG AA for large or bold text and UI parts but not for small body text. Primary buttons are fine. Small emerald text on white should use `--accent-foreground` (emerald-800) instead
- Supersedes the color part of ADR-0012
