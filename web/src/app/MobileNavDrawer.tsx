import * as DialogPrimitive from '@radix-ui/react-dialog'
import { Menu } from 'lucide-react'
import { useState } from 'react'
import { SidebarNav } from '@/app/SidebarNav'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogTitle } from '@/components/ui/dialog'

/** The collapsible sidebar for md screens: a menu button that opens the navigation as a drawer. */
export function MobileNavDrawer() {
  const [open, setOpen] = useState(false)
  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogPrimitive.Trigger asChild>
        <Button
          variant="ghost"
          size="icon"
          aria-label="Open menu"
          className="hidden md:inline-flex lg:hidden"
        >
          <Menu />
        </Button>
      </DialogPrimitive.Trigger>
      <DialogContent
        aria-describedby={undefined}
        className="top-0 left-0 h-dvh max-h-dvh w-64 max-w-64 translate-x-0 translate-y-0 rounded-none border-y-0 border-l-0"
      >
        <DialogTitle>CEM</DialogTitle>
        <SidebarNav onNavigate={() => setOpen(false)} />
      </DialogContent>
    </Dialog>
  )
}
