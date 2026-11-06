# Localization Guide

> Purpose: dates, time zones, currency and tax. Status: Draft. UI language: English only (no i18n).

## Dates — AD + Bikram Sambat
- Storage: always UTC (`timestamptz`) / AD dates (`date`)
- Display: AD, BS, or both — per tenant default + user preference (TODO)
- Conversion in one place: `lib/date` (frontend) and a backend helper for PDFs/exports
- BS date picker component (`BsDatePicker`); library TODO (evaluate `nepali-date-converter` / `nepali-datepicker`) with bounded year range
- Calendar views can show BS month grid — TODO decide scope

## Time zones
- Tenant time zone setting (default `Asia/Kathmandu`, UTC+5:45); user override TODO

## Currency
- **Single currency: NPR** (resolved). No multi-currency, no exchange rates.
- Symbol/format: `Rs.` / `रू` prefix — TODO pick; formatting via `Intl.NumberFormat` in `lib/money`
- **Digit grouping: lakh/crore** (Rs. 1,00,000 / Rs. 1,00,00,000) — use `Intl.NumberFormat('en-IN')` in `lib/money`; same rule in PDFs/exports on the backend

## Tax / VAT
- Tenant-configurable tax rates (e.g. VAT 13%); per line or per document — TODO
- Prices entered tax-inclusive or exclusive — TODO
- Quote/Invoice PDFs show tax breakdown; PAN/VAT number on documents — TODO
