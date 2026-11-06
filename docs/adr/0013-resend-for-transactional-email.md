# ADR-0013: Resend for transactional email

- Status: Accepted
- Date: 2026-11-06
- Deciders: Product owner

## Context
The app sends transactional email (reminders, notifications, share links). It must be cheap, deliverable and light on a 1 GB VPS (open question #12).

## Decision
Use **Resend** via its HTTP API, called with a plain `HttpClient` (no vendor SDK). Emails are sent from a background job, never inline in a request. Auth emails (sign-in, invites) stay with Clerk.

## Consequences
- + Simple API, free tier (~3k emails/month) fits early usage, good deliverability
- + No heavy dependency; provider sits behind an `IEmailSender` abstraction so we can swap it
- − Needs a verified sending domain (DNS records on Cloudflare)
- − Paid plan once volume grows past the free tier

## Alternatives considered
- Amazon SES — cheapest at scale, but more setup (AWS account, sandbox exit)
- Brevo — smaller free tier, marketing features we don't need
- Plain SMTP (MailKit) — provider-agnostic, but deliverability and limits vary
