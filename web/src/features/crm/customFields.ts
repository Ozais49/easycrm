import { z } from 'zod'
import type { CustomFieldDto } from '@/lib/api/generated/models'
import { formatDate } from '@/lib/date'

/** Form state of the custom fields: text, number, date and select are strings (empty = not set), checkbox is a boolean. */
export type CustomFieldFormValues = Record<string, string | boolean>

const MAX_TEXT = 1000

function fieldSchema(definition: CustomFieldDto): z.ZodType<string | boolean> {
  const required = definition.required
  switch (definition.type) {
    case 'checkbox':
      return required
        ? z.boolean().refine((checked) => checked, 'Check this box to continue.')
        : z.boolean()
    case 'number':
      return z
        .string()
        .trim()
        .refine((value) => value === '' || Number.isFinite(Number(value)), 'Enter a number.')
        .refine((value) => !required || value !== '', 'This field is required.')
    case 'date':
      return z
        .string()
        .refine((value) => value === '' || /^\d{4}-\d{2}-\d{2}$/.test(value), 'Enter a valid date.')
        .refine((value) => !required || value !== '', 'This field is required.')
    case 'select':
      return z
        .string()
        .refine((value) => value === '' || definition.options.includes(value), 'Choose an option.')
        .refine((value) => !required || value !== '', 'This field is required.')
    case 'text':
      return z
        .string()
        .trim()
        .max(MAX_TEXT, `Use ${MAX_TEXT} characters or fewer.`)
        .refine((value) => !required || value !== '', 'This field is required.')
  }
}

/** The zod schema of the custom fields part of a record form, built from the active definitions. */
export function customFieldsSchema(definitions: readonly CustomFieldDto[]) {
  const shape: Record<string, z.ZodType<string | boolean>> = {}
  for (const definition of definitions) shape[definition.key] = fieldSchema(definition)
  return z.object(shape) as unknown as z.ZodType<CustomFieldFormValues>
}

export function toCustomFieldFormValues(
  definitions: readonly CustomFieldDto[],
  values: Record<string, unknown> | undefined,
): CustomFieldFormValues {
  const result: CustomFieldFormValues = {}
  for (const definition of definitions) {
    const value = values?.[definition.key]
    if (definition.type === 'checkbox') result[definition.key] = value === true
    else if (definition.type === 'number')
      result[definition.key] = typeof value === 'number' ? String(value) : ''
    else result[definition.key] = typeof value === 'string' ? value : ''
  }
  return result
}

/** The request map: every active key is sent, null clears a value. */
export function toCustomFieldsDto(
  definitions: readonly CustomFieldDto[],
  values: CustomFieldFormValues,
): Record<string, unknown> {
  const result: Record<string, unknown> = {}
  for (const definition of definitions) {
    const value = values[definition.key]
    if (definition.type === 'checkbox') result[definition.key] = value === true
    else if (typeof value !== 'string' || value === '') result[definition.key] = null
    else result[definition.key] = definition.type === 'number' ? Number(value) : value
  }
  return result
}

/** A stored value as text for detail pages and list columns; null when there is nothing to show. */
export function formatCustomFieldValue(definition: CustomFieldDto, value: unknown): string | null {
  switch (definition.type) {
    case 'checkbox':
      return value === true ? 'Yes' : 'No'
    case 'date':
      return typeof value === 'string' && value !== '' ? formatDate(value) : null
    case 'number':
      return typeof value === 'number' ? String(value) : null
    default:
      return typeof value === 'string' && value !== '' ? value : null
  }
}

/** The error paths a server response can name, so applyFieldErrors can show them on the right input. */
export const customFieldPaths = (definitions: readonly CustomFieldDto[]) =>
  definitions.map((definition) => `customFields.${definition.key}`)

/** Label/value pairs for DetailFields, one per active definition. */
export function customFieldDetails(
  definitions: readonly CustomFieldDto[],
  values: Record<string, unknown>,
) {
  return definitions.map((definition) => ({
    label: definition.label,
    value: formatCustomFieldValue(definition, values[definition.key]),
  }))
}
