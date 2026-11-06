import { OrganizationSwitcher, UserButton } from '@clerk/react'
import { MobileNavDrawer } from '@/app/MobileNavDrawer'
import { ThemeToggle } from '@/components/ThemeToggle'

export function TopBar() {
  return (
    <header className="bg-background sticky top-0 z-20 flex h-14 items-center gap-2 border-b px-4">
      <MobileNavDrawer />
      <span className="text-lg font-semibold lg:hidden">CEM</span>
      <div className="ml-auto flex items-center gap-1">
        <OrganizationSwitcher
          hidePersonal
          afterSelectOrganizationUrl="/"
          appearance={{
            elements: {
              organizationPreviewTextContainer__organizationSwitcherTrigger: 'hidden sm:flex',
            },
          }}
        />
        <ThemeToggle />
        <UserButton />
      </div>
    </header>
  )
}
