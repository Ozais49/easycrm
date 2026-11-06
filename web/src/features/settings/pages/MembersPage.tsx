import { useState } from 'react'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { Pagination } from '@/components/Pagination'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { MemberRolesDialog } from '@/features/settings/components/MemberRolesDialog'
import { useAllRoles } from '@/features/settings/hooks/useAllRoles'
import { memberName } from '@/features/settings/memberName'
import { useListMembers } from '@/lib/api/generated/endpoints'
import type { MemberDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

export function MembersPage() {
  const [page, setPage] = useState(1)
  const [editing, setEditing] = useState<MemberDto | null>(null)
  const members = useListMembers({ page, pageSize: PAGE_SIZE })
  const roles = useAllRoles()

  const roleName = (id: string) => roles.data?.items.find((role) => role.id === id)?.name
  const columns: Column<MemberDto>[] = [
    { header: 'Name', cell: memberName },
    { header: 'Email', cell: (member) => member.email ?? '—' },
    {
      header: 'Status',
      cell: (member) => (
        <Badge variant={member.status === 'active' ? 'positive' : 'neutral'}>
          {member.status === 'active' ? 'Active' : 'Removed'}
        </Badge>
      ),
    },
    {
      header: 'Roles',
      cell: (member) => member.roleIds.map(roleName).filter(Boolean).join(', ') || '—',
    },
  ]

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Members"
        description="People in your organization and the roles they have."
      />
      {members.isPending ? <ListSkeleton /> : null}
      {members.error ? (
        <ErrorState error={members.error} onRetry={() => void members.refetch()} />
      ) : null}
      {members.data && members.data.items.length === 0 ? (
        <EmptyState message="No members yet. Invite people from the organization menu in the top bar." />
      ) : null}
      {members.data && members.data.items.length > 0 ? (
        <>
          <ResponsiveTable
            items={members.data.items}
            getKey={(member) => member.membershipId}
            columns={columns}
            actions={(member) => (
              <PermissionGate scope="settings" action="edit">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!roles.data}
                  onClick={() => setEditing(member)}
                >
                  Edit roles
                </Button>
              </PermissionGate>
            )}
          />
          <Pagination
            page={members.data.page}
            pageSize={members.data.pageSize}
            total={members.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}
      {editing && roles.data ? (
        <MemberRolesDialog
          key={editing.membershipId}
          member={editing}
          roles={roles.data.items}
          onClose={() => setEditing(null)}
        />
      ) : null}
    </div>
  )
}
