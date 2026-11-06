import { Button } from '@/components/ui/button'
import { friendlyMessage } from '@/lib/api/errors'

type ErrorStateProps = {
  error: unknown
  onRetry?: () => void
}

export function ErrorState({ error, onRetry }: ErrorStateProps) {
  return (
    <div
      role="alert"
      className="flex flex-col items-center gap-3 rounded-lg border p-8 text-center"
    >
      <p>{friendlyMessage(error)}</p>
      {onRetry ? (
        <Button variant="outline" onClick={onRetry}>
          Try again
        </Button>
      ) : null}
    </div>
  )
}
