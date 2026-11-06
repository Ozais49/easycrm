import type { FieldValues, Path, UseFormSetError } from 'react-hook-form'
import { ApiError } from './errors'

/**
 * Copies the server's field errors onto the form. Server keys are property names ("TimeZone"),
 * form fields are camelCase. Returns false when nothing could be mapped, so the caller can toast instead.
 */
export function applyFieldErrors<T extends FieldValues>(
  error: unknown,
  setError: UseFormSetError<T>,
  fields: readonly Path<T>[],
): boolean {
  if (!(error instanceof ApiError)) return false
  let mapped = false
  for (const [key, messages] of Object.entries(error.fieldErrors)) {
    const field = fields.find((f) => f.toLowerCase() === key.toLowerCase())
    const message = messages[0]
    if (field && message) {
      setError(field, { message })
      mapped = true
    }
  }
  return mapped
}
