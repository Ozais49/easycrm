import { TagColor } from '@/lib/api/generated/models'

/** The fixed palette of a tag: token names, mapped to colors by the Badge variants. */
export const TAG_COLORS = [
  TagColor.neutral,
  TagColor.informative,
  TagColor.positive,
  TagColor.notice,
  TagColor.negative,
] as const

export const TAG_COLOR_LABELS: Record<TagColor, string> = {
  neutral: 'Neutral',
  informative: 'Informative',
  positive: 'Positive',
  notice: 'Notice',
  negative: 'Negative',
}
