import { CustomFieldType } from '@/lib/api/generated/models'

export const CUSTOM_FIELD_TYPES = [
  CustomFieldType.text,
  CustomFieldType.number,
  CustomFieldType.date,
  CustomFieldType.select,
  CustomFieldType.checkbox,
] as const

export const CUSTOM_FIELD_TYPE_LABELS: Record<CustomFieldType, string> = {
  text: 'Text',
  number: 'Number',
  date: 'Date',
  select: 'Select (one of a list)',
  checkbox: 'Checkbox',
}
