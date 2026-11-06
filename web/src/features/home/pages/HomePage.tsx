import { Link } from '@tanstack/react-router'
import { EmptyState } from '@/components/EmptyState'
import { PageHeader } from '@/components/PageHeader'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useMe } from '@/features/identity/hooks/useMe'

export function HomePage() {
  const { data: me, isPending } = useMe()

  return (
    <div className="flex flex-col gap-6">
      {isPending ? (
        <Skeleton className="h-9 w-64" />
      ) : (
        <PageHeader title={me?.user.firstName ? `Welcome, ${me.user.firstName}` : 'Welcome'} />
      )}
      <EmptyState
        message="Nothing to show yet. Your work will appear here as modules are added."
        action={
          <Button variant="outline" asChild>
            <Link to="/settings">Review settings</Link>
          </Button>
        }
      />
    </div>
  )
}
