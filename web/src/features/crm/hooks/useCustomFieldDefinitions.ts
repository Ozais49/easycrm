import { useListCustomFields } from '@/lib/api/generated/endpoints'
import type { CustomFieldEntity } from '@/lib/api/generated/models'

/** The active custom fields of an entity type in display order. Open to every member. */
export function useCustomFieldDefinitions(entityType: CustomFieldEntity) {
  return useListCustomFields({ entityType })
}
