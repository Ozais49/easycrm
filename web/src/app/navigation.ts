import { Home, Settings, type LucideIcon } from 'lucide-react'

export type NavItem = {
  to: '/' | '/settings'
  label: string
  icon: LucideIcon
}

/**
 * Only modules that exist. Settings is open to every member because General is read-only for them;
 * the admin pages inside it are gated by permission.
 */
export const NAV_ITEMS: readonly NavItem[] = [
  { to: '/', label: 'Home', icon: Home },
  { to: '/settings', label: 'Settings', icon: Settings },
]
