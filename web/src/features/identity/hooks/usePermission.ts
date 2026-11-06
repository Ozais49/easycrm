import { useMe } from '@/features/identity/hooks/useMe'
import type { Action, Scope } from '@/lib/permissions'

/**
 * True when the user has any level of the permission, per the cached /api/me.
 * Cosmetic only: it decides what to show. The server enforces every permission.
 */
export function usePermission(scope: Scope, action: Action): boolean {
  const { data } = useMe()
  const level = data?.permissions.scopes?.[scope]?.[action]
  return level !== undefined && level !== 'none'
}
