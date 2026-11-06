# Integrations

> Purpose: external channel integrations. Status: Draft

## Abstraction
`IChannelProvider { ParseWebhook, SendMessage, RefreshToken, GetAccountInfo }` per channel. Inbound messages → `Conversation` / `Message` rows + `MessageReceived` event. Dedupe by external message id.

## WhatsApp Business (Cloud API)
- Webhook `GET/POST /webhooks/meta` (verify token + `X-Hub-Signature-256`)
- Send via Graph API `/{phone_number_id}/messages`; 24h service window, templates outside it
- Tenant resolved by `phone_number_id`

## Instagram (Instagram API with Instagram Login)
- Business/creator accounts; DMs via the same Meta webhook (comments — TODO)
- Meta App Review required for production; build against dev mode + test users

## TikTok
- v1: Lead Generation (instant form leads → Lead) + basic account stats
- DM messaging deferred (API access restricted)

## Fallbacks
- `wa.me` click-to-chat links and profile links on Contacts

## TODO
- Meta app setup checklist, token lifecycle, media messages, retries/idempotency
