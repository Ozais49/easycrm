import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { GeneralForm } from '@/features/settings/components/GeneralForm'
import { useTenantSettings } from '@/features/settings/hooks/useTenantSettings'

export function GeneralPage() {
  const { data, error, isPending, refetch } = useTenantSettings()

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="General"
        description="Time zone, date display and brand color for your organization."
      />
      {isPending ? <ListSkeleton rows={3} /> : null}
      {error ? <ErrorState error={error} onRetry={() => void refetch()} /> : null}
      {data ? <GeneralForm settings={data} /> : null}
    </div>
  )
}
