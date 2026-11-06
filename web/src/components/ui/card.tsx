import type { ComponentProps } from 'react'
import { cn } from '@/lib/utils'

export function Card({ className, ...props }: ComponentProps<'div'>) {
  return (
    <div
      className={cn(
        'bg-card text-card-foreground flex flex-col gap-3 rounded-lg border p-4',
        className,
      )}
      {...props}
    />
  )
}
