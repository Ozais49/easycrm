import { Building2, Home, Settings, Users, type LucideIcon } from 'lucide-react'
import { useMe } from '@/features/identity/hooks/useMe'
import type { Action, Scope } from '@/lib/permissions'

export type NavItem = {
  to: '/' | '/companies' | '/contacts' | '/settings'
  label: string
  icon: LucideIcon
  /** Shown only when the user has any level of this permission; always shown when omitted. */
  permission?: { scope: Scope; action: Action }
}

/**
 * Only modules that exist. Settings is open to every member because General is read-only for them;
 * the admin pages inside it are gated by permission.
 */
export const NAV_ITEMS: readonly NavItem[] = [
  { to: '/', label: 'Home', icon: Home },
  {
    to: '/companies',
    label: 'Companies',
    icon: Building2,
    permission: { scope: 'crm', action: 'read' },
  },
  {
    to: '/contacts',
    label: 'Contacts',
    icon: Users,
    permission: { scope: 'crm', action: 'read' },
  },
  { to: '/settings', label: 'Settings', icon: Settings },
]

/** The navigation items the user may use, per the cached /api/me (cosmetic: the server enforces access). */
export function useNavItems(): readonly NavItem[] {
  const { data } = useMe()
  return NAV_ITEMS.filter(({ permission }) => {
    if (!permission) return true
    const level = data?.permissions.scopes?.[permission.scope]?.[permission.action]
    return level !== undefined && level !== 'none'
  })
}
