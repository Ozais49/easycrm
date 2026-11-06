import { Outlet } from '@tanstack/react-router'
import { EmptyState } from '@/components/EmptyState'
import { PermissionGate } from '@/components/PermissionGate'

/** Members, roles, teams and the audit log are for people with Settings:Read. */
export function AdminGuard() {
  return (
    <PermissionGate
      scope="settings"
      action="read"
      fallback={
        <EmptyState message="You don't have access to this page. Ask an admin for access." />
      }
    >
      <Outlet />
    </PermissionGate>
  )
}
