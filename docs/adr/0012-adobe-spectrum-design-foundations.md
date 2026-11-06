# ADR-0012: Adobe Spectrum as design foundations

- Status: Accepted
- Date: 2026-11-06
- Deciders: Product owner

## Context
Brand color, typography and visual rules were placeholders (open question #2). We need a mature, documented design system to follow so the UI is consistent without inventing our own foundations.

## Decision
Follow [Adobe Spectrum](https://spectrum.adobe.com/) as the **design guidelines** for foundations: color system, typography scale, spacing, sizing/platform scale, iconography rules, states, motion, accessibility and writing. The **implementation stays shadcn/ui + Tailwind v4** ([ADR-0007](0007-shadcn-tailwind-ui.md)); Spectrum values are expressed as our Tailwind/CSS tokens and shadcn components are styled to match.

We do **not** adopt React Spectrum, Spectrum CSS or React Aria components.

## Consequences
- + Proven, documented foundations; fewer ad-hoc design decisions
- + Keeps ADR-0007 benefits: light bundle, full control, easy per-tenant `--primary` override
- − Components only approximate Spectrum behaviour; we map Spectrum guidance onto shadcn ourselves
- − Adobe Clean font is proprietary and not used; we use **Source Sans 3** + **Source Code Pro** instead (Adobe open source, SIL OFL, free for commercial use), self-hosted (see UI design guide)
- Follow-up: map Spectrum tokens to our CSS variables when scaffolding `web/`

## Alternatives considered
- React Spectrum S2 — most faithful, but locked-down styling makes tenant branding hard, and the bundle is heavier
- React Aria Components + Tailwind — good fidelity, but means replacing shadcn/Radix
