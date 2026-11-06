# ADR-0008: Responsive web only (no PWA, no native app)

- Status: Proposed
- Date: 2026-11-06

## Context
Users need the CRM on phones and desktops. Options were PWA, Capacitor wrapper, or a native Expo app.

## Decision
Ship a responsive web app only. No service worker, no install prompt, no native wrapper for now. Keep the API client-agnostic so a mobile app could be added later.

## Consequences
- + Smallest frontend scope; one codebase
- − No offline support, no native push notifications
- Revisit if offline use or push becomes a requirement

## Alternatives considered
- PWA — install + push, modest extra effort
- Capacitor — store presence
- Expo native — roughly doubles frontend work
