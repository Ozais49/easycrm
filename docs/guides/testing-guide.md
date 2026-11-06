# Testing Guide

> Purpose: test & verification strategy. Status: Accepted — [ADR-0010](../adr/0010-no-automated-tests-initially.md)

## Current policy
**No automated tests for now.** Do not create test projects unless asked.

## Manual verification per feature
1. Feature implemented on a feature branch → PR
2. Product owner verifies manually (desktop + mobile width, light + dark mode)
3. Deployed
4. Status/feedback recorded (PR comment or issue) → follow-up fixes

## Self-check before handing a feature over
- Tenant isolation: data from another tenant never appears
- Roles without the scope can't see the module or call its endpoints
- Sensitive fields (pricing/financial/PII) absent for roles without permission
- Works at 360px width and in dark mode
- No new heavy dependency without noting memory impact ([ADR-0009](../adr/0009-cloudflare-plus-low-end-vps.md))

## Later (when revisited)
Tenant-isolation and permission integration tests first, before onboarding a second tenant.
