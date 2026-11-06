import { Link } from '@tanstack/react-router'
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
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { toastApiError } from '@/lib/api/errors'
import { getListRolesQueryKey, useDeleteRole, useListRoles } from '@/lib/api/generated/endpoints'
import type { RoleDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

const columns: Column<RoleDto>[] = [
  { header: 'Name', cell: (role) => role.name },
  {
    header: 'Type',
    cell: (role) => <Badge>{role.isSystem ? 'Built-in' : 'Custom'}</Badge>,
  },
]

export function RolesPage() {
  const queryClient = useQueryClient()
  const [page, setPage] = useState(1)
  const [deleting, setDeleting] = useState<RoleDto | null>(null)
  const roles = useListRoles({ page, pageSize: PAGE_SIZE })
  const deleteRole = useDeleteRole()

  const confirmDelete = () => {
    if (!deleting) return
    deleteRole.mutate(
      { id: deleting.id },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListRolesQueryKey() })
          toast.success(`Role '${deleting.name}' deleted.`)
          setDeleting(null)
        },
        onError: (error) => {
          setDeleting(null)
          toastApiError(error)
        },
      },
    )
  }

  const newRoleButton = (
    <PermissionGate scope="settings" action="edit">
      <Button asChild>
        <Link to="/settings/roles/new">New role</Link>
      </Button>
    </PermissionGate>
  )

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Roles"
        description="Roles decide what members can see and do."
        action={newRoleButton}
      />
      {roles.isPending ? <ListSkeleton /> : null}
      {roles.error ? <ErrorState error={roles.error} onRetry={() => void roles.refetch()} /> : null}
      {roles.data && roles.data.items.length === 0 ? (
        <EmptyState message="No roles yet. Add the first one." action={newRoleButton} />
      ) : null}
      {roles.data && roles.data.items.length > 0 ? (
        <>
          <ResponsiveTable
            items={roles.data.items}
            getKey={(role) => role.id}
            columns={columns}
            actions={(role) => (
              <>
                <Button variant="outline" size="sm" asChild>
                  <Link to="/settings/roles/$roleId" params={{ roleId: role.id }}>
                    <PermissionGate scope="settings" action="edit" fallback="View">
                      Edit
                    </PermissionGate>
                  </Link>
                </Button>
                {role.isSystem ? null : (
                  <PermissionGate scope="settings" action="delete">
                    <Button variant="outline" size="sm" onClick={() => setDeleting(role)}>
                      Delete
                    </Button>
                  </PermissionGate>
                )}
              </>
            )}
          />
          <Pagination
            page={roles.data.page}
            pageSize={roles.data.pageSize}
            total={roles.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}
      <ConfirmDialog
        open={deleting !== null}
        onOpenChange={(open) => (open ? undefined : setDeleting(null))}
        title={`Delete role '${deleting?.name ?? ''}'?`}
        description="Members keep their other roles. This can't be undone."
        confirmLabel="Delete role"
        pending={deleteRole.isPending}
        onConfirm={confirmDelete}
      />
    </div>
  )
}
