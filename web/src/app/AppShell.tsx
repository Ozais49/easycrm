import { Outlet } from '@tanstack/react-router'
import { BottomTabs } from '@/app/BottomTabs'
import { BrandApplier } from '@/app/BrandApplier'
import { SidebarNav } from '@/app/SidebarNav'
import { TopBar } from '@/app/TopBar'

/** Sidebar on lg+ (a drawer on md), top bar everywhere, bottom tabs on mobile. */
export function AppShell() {
  return (
    <div className="min-h-dvh lg:grid lg:grid-cols-[15rem_1fr]">
      <BrandApplier />
      <aside className="bg-sidebar sticky top-0 hidden h-dvh flex-col gap-4 border-r p-4 lg:flex">
        <span className="px-3 text-xl font-semibold">CEM</span>
        <SidebarNav />
      </aside>
      <div className="flex min-w-0 flex-col">
        <TopBar />
        <main className="mx-auto w-full max-w-5xl flex-1 p-4 pb-24 md:p-6 md:pb-6">
          <Outlet />
        </main>
      </div>
      <BottomTabs />
    </div>
  )
}
