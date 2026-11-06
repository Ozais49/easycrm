/**
 * Date display lives here (localization-guide). Only AD is rendered so far: the BS converter is still an
 * open decision, so the tenant's AD / BS / Both preference is not applied yet.
 */
export function formatDateTime(iso: string, timeZone: string): string {
  try {
    return new Intl.DateTimeFormat('en-GB', {
      dateStyle: 'medium',
      timeStyle: 'short',
      timeZone,
    }).format(new Date(iso))
  } catch {
    // An unknown zone id: show the time in the browser's zone instead of failing.
    return new Date(iso).toLocaleString('en-GB')
  }
}
