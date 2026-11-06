import { useState } from 'react'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { Pagination } from '@/components/Pagination'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Badge } from '@/components/ui/badge'
import { AuditChanges } from '@/features/settings/components/AuditChanges'
import { useTenantSettings } from '@/features/settings/hooks/useTenantSettings'
import { formatDateTime } from '@/lib/date'
import { useListAuditLog } from '@/lib/api/generated/endpoints'
import type { AuditEntryDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

const ACTION_BADGES = {
  created: { label: 'Created', variant: 'positive' },
  updated: { label: 'Updated', variant: 'informative' },
  deleted: { label: 'Deleted', variant: 'negative' },
} as const

export function AuditLogPage() {
  const [page, setPage] = useState(1)
  const log = useListAuditLog({ page, pageSize: PAGE_SIZE })
  const { data: settings } = useTenantSettings()
  const timeZone = settings?.timeZone ?? 'UTC'

  const columns: Column<AuditEntryDto>[] = [
    { header: 'When', cell: (entry) => formatDateTime(entry.occurredAt, timeZone) },
    {
      // The user's name isn't available yet; the id is shown for now, or System for automatic changes.
      header: 'Who',
      cell: (entry) =>
        entry.userId ? (
          <span className="font-mono text-xs break-all">{entry.userId}</span>
        ) : (
          'System'
        ),
    },
    {
      header: 'Action',
      cell: ({ action }) => (
        <Badge variant={ACTION_BADGES[action].variant}>{ACTION_BADGES[action].label}</Badge>
      ),
    },
    {
      header: 'Type',
      cell: (entry) => <span className="font-mono text-xs">{entry.entityType}</span>,
    },
    { header: 'Changes', cell: (entry) => <AuditChanges changes={entry.changes} /> },
  ]

  return (
    <div className="flex flex-col gap-6">
      <PageHeader title="Audit log" description="Who changed what, newest first." />
      {log.isPending ? <ListSkeleton /> : null}
      {log.error ? <ErrorState error={log.error} onRetry={() => void log.refetch()} /> : null}
      {log.data && log.data.items.length === 0 ? (
        <EmptyState message="No changes recorded yet. They'll show up here as people edit data." />
      ) : null}
      {log.data && log.data.items.length > 0 ? (
        <>
          <ResponsiveTable items={log.data.items} getKey={(entry) => entry.id} columns={columns} />
          <Pagination
            page={log.data.page}
            pageSize={log.data.pageSize}
            total={log.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}
    </div>
  )
}
