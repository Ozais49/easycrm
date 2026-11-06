export type ChangedField = {
  field: string
  /** "old → new", or "Hidden" when the value is redacted. */
  summary: string
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null
}

function show(value: unknown): string {
  if (value === null || value === undefined) return 'empty'
  return typeof value === 'string' ? value : JSON.stringify(value)
}

/** Reads the audit `changes` JSON: `{ field: { old, new } }`, or `{ field: { redacted: true } }` for sensitive values. */
export function parseChanges(changes: unknown): ChangedField[] {
  if (!isRecord(changes)) return []
  return Object.entries(changes).map(([field, change]) => {
    if (!isRecord(change) || change.redacted === true) return { field, summary: 'Hidden' }
    const summary = 'old' in change ? `${show(change.old)} → ${show(change.new)}` : show(change.new)
    return { field, summary }
  })
}
