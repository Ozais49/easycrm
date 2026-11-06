import type { ReactNode } from 'react'
import { usePermission } from '@/features/identity/hooks/usePermission'
import type { Action, Scope } from '@/lib/permissions'

type PermissionGateProps = {
  scope: Scope
  action: Action
  /** Shown instead of the children; nothing by default (hide, don't disable). */
  fallback?: ReactNode
  children: ReactNode
}

/** Hides what the user cannot use. Cosmetic: the server enforces every permission. */
export function PermissionGate({ scope, action, fallback = null, children }: PermissionGateProps) {
  return usePermission(scope, action) ? children : fallback
}
