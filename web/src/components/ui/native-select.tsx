import { ChevronDown } from 'lucide-react'
import type { ComponentProps } from 'react'
import { cn } from '@/lib/utils'

/** A styled native select: best touch behaviour on phones and no extra dependency. */
export function NativeSelect({ className, children, ...props }: ComponentProps<'select'>) {
  return (
    <div className="relative w-full">
      <select
        className={cn(
          'border-input bg-background h-11 w-full appearance-none rounded-md border pr-9 pl-3 text-base disabled:cursor-not-allowed disabled:opacity-60 md:h-9 md:text-sm',
          className,
        )}
        {...props}
      >
        {children}
      </select>
      <ChevronDown className="text-muted-foreground pointer-events-none absolute top-1/2 right-3 size-4 -translate-y-1/2" />
    </div>
  )
}
