import type { ErrorComponentProps } from '@tanstack/react-router'
import { ErrorState } from '@/components/ErrorState'

export function RouteError({ error, reset }: ErrorComponentProps) {
  return (
    <div className="mx-auto max-w-md p-6">
      <ErrorState error={error} onRetry={reset} />
    </div>
  )
}
