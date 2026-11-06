import { Link } from '@tanstack/react-router'
import { EmptyState } from '@/components/EmptyState'
import { Button } from '@/components/ui/button'

export function RouteNotFound() {
  return (
    <EmptyState
      message="We couldn't find that page. Check the address or go back home."
      action={
        <Button asChild>
          <Link to="/">Go to Home</Link>
        </Button>
      }
    />
  )
}
