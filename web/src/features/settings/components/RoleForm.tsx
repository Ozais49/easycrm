import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate } from '@tanstack/react-router'
import { useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { FormField } from '@/components/FormField'
import { PermissionGate } from '@/components/PermissionGate'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { usePermission } from '@/features/identity/hooks/usePermission'
import { FieldGroupMatrix } from '@/features/settings/components/FieldGroupMatrix'
import { ScopeMatrix } from '@/features/settings/components/ScopeMatrix'
import {
  roleSchema,
  toPermissionMatrix,
  toRoleFormValues,
  type RoleFormValues,
} from '@/features/settings/roleForm'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import {
  getGetRoleQueryKey,
  getListRolesQueryKey,
  useCreateRole,
  useUpdateRole,
} from '@/lib/api/generated/endpoints'
import type { RoleDto } from '@/lib/api/generated/models'

const FIELDS = ['name'] as const

/** Creates a role, or edits the name and permission matrix of an existing one. */
export function RoleForm({ role }: { role?: RoleDto }) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()
  const canEdit = usePermission('settings', 'edit')
  const create = useCreateRole()
  const update = useUpdateRole()
  const form = useForm<RoleFormValues>({
    resolver: zodResolver(roleSchema),
    defaultValues: toRoleFormValues(role?.name ?? '', role?.permissions),
  })
  const pending = create.isPending || update.isPending

  const onSaved = async () => {
    await queryClient.invalidateQueries({ queryKey: getListRolesQueryKey() })
    if (role) await queryClient.invalidateQueries({ queryKey: getGetRoleQueryKey(role.id) })
    toast.success(role ? 'Role saved.' : 'Role created.')
    await navigate({ to: '/settings/roles' })
  }
  const onError = (error: unknown) => {
    if (!applyFieldErrors(error, form.setError, FIELDS)) toastApiError(error)
  }

  const onSubmit = form.handleSubmit((values) => {
    const data = { name: values.name, permissions: toPermissionMatrix(values) }
    if (role) update.mutate({ id: role.id, data }, { onSuccess: onSaved, onError })
    else create.mutate({ data }, { onSuccess: onSaved, onError })
  })

  return (
    <form onSubmit={onSubmit} className="flex flex-col gap-8" noValidate>
      <div className="max-w-md">
        <FormField label="Role name" htmlFor="name" error={form.formState.errors.name?.message}>
          <Input
            id="name"
            disabled={!canEdit}
            aria-invalid={form.formState.errors.name ? true : undefined}
            {...form.register('name')}
          />
        </FormField>
      </div>

      <section className="flex flex-col gap-3" aria-labelledby="areas-heading">
        <h2 id="areas-heading" className="text-xl font-semibold">
          What this role can do
        </h2>
        <p className="text-muted-foreground text-sm">
          None, own records, team records or all records, for each action.
        </p>
        <ScopeMatrix form={form} disabled={!canEdit} />
      </section>

      <section className="flex flex-col gap-3" aria-labelledby="fields-heading">
        <h2 id="fields-heading" className="text-xl font-semibold">
          Sensitive details
        </h2>
        <p className="text-muted-foreground text-sm">
          Who can see and change these details on records.
        </p>
        <FieldGroupMatrix form={form} disabled={!canEdit} />
      </section>

      <div className="flex flex-col-reverse gap-2 sm:flex-row">
        <Button variant="outline" asChild>
          <Link to="/settings/roles">{canEdit ? 'Cancel' : 'Back to roles'}</Link>
        </Button>
        <PermissionGate scope="settings" action="edit">
          <Button type="submit" disabled={pending}>
            {role ? 'Save role' : 'Create role'}
          </Button>
        </PermissionGate>
      </div>
    </form>
  )
}
