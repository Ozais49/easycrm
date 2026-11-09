import { Link } from '@tanstack/react-router'
import { useNavItems } from '@/app/navigation'

type SidebarNavProps = {
  onNavigate?: () => void
}

export function SidebarNav({ onNavigate }: SidebarNavProps) {
  const navItems = useNavItems()
  return (
    <nav aria-label="Main" className="flex flex-col gap-1">
      {navItems.map(({ to, label, icon: Icon }) => (
        <Link
          key={to}
          to={to}
          activeOptions={{ exact: to === '/' }}
          onClick={onNavigate}
          className="text-muted-foreground hover:bg-accent hover:text-accent-foreground data-[status=active]:bg-accent data-[status=active]:text-accent-foreground flex h-11 items-center gap-3 rounded-md px-3 text-base font-semibold md:h-9 md:text-sm"
        >
          <Icon className="size-4" />
          {label}
        </Link>
      ))}
    </nav>
  )
}
