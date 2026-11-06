import type { ReactNode } from 'react'

type PageHeaderProps = {
  title: string
  description?: string
  /** The one primary action of the screen. */
  action?: ReactNode
}

export function PageHeader({ title, description, action }: PageHeaderProps) {
  return (
    <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
      <div className="min-w-0">
        <h1 className="text-3xl font-semibold">{title}</h1>
        {description ? <p className="text-muted-foreground mt-1">{description}</p> : null}
      </div>
      {action}
    </div>
  )
}
