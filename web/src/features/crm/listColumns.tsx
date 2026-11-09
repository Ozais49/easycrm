import type { Column } from '@/components/ResponsiveTable'
import { TagBadges } from '@/features/crm/components/TagBadges'
import { formatCustomFieldValue } from '@/features/crm/customFields'
import type { CustomFieldDto, TagDto } from '@/lib/api/generated/models'

type Tagged = { tags: TagDto[] }
type WithCustomFields = { customFields: Record<string, unknown> }

/** Number of flagged custom fields a mobile card shows; the table shows all of them. */
const MOBILE_CUSTOM_FIELDS = 2

export function tagsColumn<T extends Tagged>(): Column<T> {
  return { header: 'Tags', cell: (item) => <TagBadges tags={item.tags} /> }
}

/** One column per custom field flagged "show in list". */
export function customFieldColumns<T extends WithCustomFields>(
  definitions: readonly CustomFieldDto[] | undefined,
): Column<T>[] {
  return (definitions ?? [])
    .filter((definition) => definition.showInList)
    .map((definition, index) => ({
      header: definition.label,
      hideOnMobile: index >= MOBILE_CUSTOM_FIELDS,
      cell: (item: T) =>
        formatCustomFieldValue(definition, item.customFields[definition.key]) ?? '—',
    }))
}
