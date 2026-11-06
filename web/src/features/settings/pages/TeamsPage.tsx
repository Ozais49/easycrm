import { useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { Pagination } from '@/components/Pagination'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Button } from '@/components/ui/button'
import { TeamMembersDialog } from '@/features/settings/components/TeamMembersDialog'
import { TeamNameDialog } from '@/features/settings/components/TeamNameDialog'
import { toastApiError } from '@/lib/api/errors'
import { getListTeamsQueryKey, useDeleteTeam, useListTeams } from '@/lib/api/generated/endpoints'
import type { TeamDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

const columns: Column<TeamDto>[] = [
  { header: 'Name', cell: (team) => team.name },
  { header: 'Members', cell: (team) => team.membershipIds.length },
]

type ActiveDialog =
  | { kind: 'create' }
  | { kind: 'rename'; team: TeamDto }
  | { kind: 'members'; team: TeamDto }
  | { kind: 'delete'; team: TeamDto }

export function TeamsPage() {
  const queryClient = useQueryClient()
  const [page, setPage] = useState(1)
  const [dialog, setDialog] = useState<ActiveDialog | null>(null)
  const teams = useListTeams({ page, pageSize: PAGE_SIZE })
  const deleteTeam = useDeleteTeam()
  const close = () => setDialog(null)

  const confirmDelete = (team: TeamDto) =>
    deleteTeam.mutate(
      { id: team.id },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListTeamsQueryKey() })
          toast.success(`Team '${team.name}' deleted.`)
          close()
        },
        onError: (error) => {
          close()
          toastApiError(error)
        },
      },
    )

  const newTeamButton = (
    <PermissionGate scope="settings" action="edit">
      <Button onClick={() => setDialog({ kind: 'create' })}>New team</Button>
    </PermissionGate>
  )

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Teams"
        description="Group members so records can be shared with a team."
        action={newTeamButton}
      />
      {teams.isPending ? <ListSkeleton /> : null}
      {teams.error ? <ErrorState error={teams.error} onRetry={() => void teams.refetch()} /> : null}
      {teams.data && teams.data.items.length === 0 ? (
        <EmptyState message="No teams yet. Add the first one." action={newTeamButton} />
      ) : null}
      {teams.data && teams.data.items.length > 0 ? (
        <>
          <ResponsiveTable
            items={teams.data.items}
            getKey={(team) => team.id}
            columns={columns}
            actions={(team) => (
              <>
                <PermissionGate scope="settings" action="edit">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDialog({ kind: 'members', team })}
                  >
                    Members
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDialog({ kind: 'rename', team })}
                  >
                    Rename
                  </Button>
                </PermissionGate>
                <PermissionGate scope="settings" action="delete">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDialog({ kind: 'delete', team })}
                  >
                    Delete
                  </Button>
                </PermissionGate>
              </>
            )}
          />
          <Pagination
            page={teams.data.page}
            pageSize={teams.data.pageSize}
            total={teams.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}

      {dialog?.kind === 'create' ? <TeamNameDialog onClose={close} /> : null}
      {dialog?.kind === 'rename' ? <TeamNameDialog team={dialog.team} onClose={close} /> : null}
      {dialog?.kind === 'members' ? <TeamMembersDialog team={dialog.team} onClose={close} /> : null}
      <ConfirmDialog
        open={dialog?.kind === 'delete'}
        onOpenChange={(open) => (open ? undefined : close())}
        title={`Delete team '${dialog?.kind === 'delete' ? dialog.team.name : ''}'?`}
        description="Members stay in your organization. This can't be undone."
        confirmLabel="Delete team"
        pending={deleteTeam.isPending}
        onConfirm={() => (dialog?.kind === 'delete' ? confirmDelete(dialog.team) : undefined)}
      />
    </div>
  )
}
