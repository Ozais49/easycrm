import type { ReactNode } from 'react'

type FullPageMessageProps = {
  title: string
  children?: ReactNode
  action?: ReactNode
}

/** A centered full-page message for states outside the app shell (setup missing, crash). */
export function FullPageMessage({ title, children, action }: FullPageMessageProps) {
  return (
    <main className="flex min-h-dvh items-center justify-center p-4">
      <div className="flex max-w-md flex-col items-center gap-3 text-center">
        <h1 className="text-2xl font-semibold">{title}</h1>
        {children ? <p className="text-muted-foreground">{children}</p> : null}
        {action}
      </div>
    </main>
  )
}
