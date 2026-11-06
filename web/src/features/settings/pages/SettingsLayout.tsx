import { Link, Outlet } from '@tanstack/react-router'
import { PermissionGate } from '@/components/PermissionGate'

const TAB_CLASS =
  'text-muted-foreground hover:text-foreground data-[status=active]:border-primary data-[status=active]:text-foreground flex h-11 items-center border-b-2 border-transparent px-3 text-base font-semibold whitespace-nowrap md:text-sm'

/** Settings frame: sub-navigation (scrolls sideways on narrow screens) above the active page. */
export function SettingsLayout() {
  return (
    <div className="flex flex-col gap-6">
      <nav aria-label="Settings" className="-mx-4 overflow-x-auto border-b px-4 md:mx-0 md:px-0">
        <div className="flex">
          <Link to="/settings/general" className={TAB_CLASS}>
            General
          </Link>
          <PermissionGate scope="settings" action="read">
            <Link to="/settings/members" className={TAB_CLASS}>
              Members
            </Link>
            <Link to="/settings/roles" className={TAB_CLASS}>
              Roles
            </Link>
            <Link to="/settings/teams" className={TAB_CLASS}>
              Teams
            </Link>
            <Link to="/settings/audit-log" className={TAB_CLASS}>
              Audit log
            </Link>
          </PermissionGate>
        </div>
      </nav>
      <Outlet />
    </div>
  )
}
