import { useMe } from '@/features/identity/hooks/useMe'
import type { FieldGroup } from '@/lib/permissions'

/**
 * Whether the user may read or edit a sensitive field group, per the cached /api/me.
 * Cosmetic only: the server omits unreadable fields and rejects forbidden writes.
 */
export function useFieldPermission(group: FieldGroup): { canRead: boolean; canEdit: boolean } {
  const { data } = useMe()
  const access = data?.permissions.fields?.[group]
  return { canRead: access?.read === true, canEdit: access?.edit === true }
}
