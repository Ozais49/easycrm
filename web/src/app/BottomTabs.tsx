import { Link } from '@tanstack/react-router'
import { useNavItems } from '@/app/navigation'

/** Mobile navigation; hidden from md up, where the sidebar takes over. */
export function BottomTabs() {
  const navItems = useNavItems()
  return (
    <nav
      aria-label="Main"
      className="bg-background fixed inset-x-0 bottom-0 z-30 flex border-t pb-[env(safe-area-inset-bottom)] md:hidden"
    >
      {navItems.map(({ to, label, icon: Icon }) => (
        <Link
          key={to}
          to={to}
          activeOptions={{ exact: to === '/' }}
          className="text-muted-foreground data-[status=active]:text-primary flex min-h-14 flex-1 flex-col items-center justify-center gap-0.5 text-xs font-semibold"
        >
          <Icon className="size-5" />
          {label}
        </Link>
      ))}
    </nav>
  )
}
