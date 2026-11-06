import type { ComponentProps } from 'react'
import { cn } from '@/lib/utils'

export function Input({ className, type, ...props }: ComponentProps<'input'>) {
  return (
    <input
      type={type}
      className={cn(
        'border-input bg-background placeholder:text-muted-foreground h-11 w-full min-w-0 rounded-md border px-3 text-base transition-colors disabled:cursor-not-allowed disabled:opacity-60 aria-invalid:border-destructive md:h-9 md:text-sm',
        className,
      )}
      {...props}
    />
  )
}
