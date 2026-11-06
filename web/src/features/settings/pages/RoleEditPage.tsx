import { useParams } from '@tanstack/react-router'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { RoleForm } from '@/features/settings/components/RoleForm'
import { useGetRole } from '@/lib/api/generated/endpoints'

export function RoleEditPage() {
  const { roleId } = useParams({ from: '/settings/admin/roles/$roleId' })
  const { data: role, error, isPending, refetch } = useGetRole(roleId)

  return (
    <div className="flex flex-col gap-6">
      <PageHeader title={role ? `Role: ${role.name}` : 'Role'} />
      {isPending ? <ListSkeleton rows={5} /> : null}
      {error ? <ErrorState error={error} onRetry={() => void refetch()} /> : null}
      {role ? <RoleForm key={role.id} role={role} /> : null}
    </div>
  )
}
