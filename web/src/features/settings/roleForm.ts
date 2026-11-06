import { z } from 'zod'
import type { PermissionMatrix } from '@/lib/api/generated/models'
import { ACTIONS, FIELD_GROUPS, SCOPES } from '@/lib/permissions'

export const LEVELS = ['none', 'own', 'team', 'all'] as const

export const roleSchema = z.object({
  name: z.string().trim().min(1, 'Enter a role name.').max(100, 'Use 100 characters or fewer.'),
  scopes: z.record(z.enum(SCOPES), z.record(z.enum(ACTIONS), z.enum(LEVELS))),
  fields: z.record(z.enum(FIELD_GROUPS), z.object({ read: z.boolean(), edit: z.boolean() })),
})

export type RoleFormValues = z.infer<typeof roleSchema>

/** Every cell filled in, so the form always edits a complete matrix. */
export function toRoleFormValues(name: string, matrix?: PermissionMatrix): RoleFormValues {
  const scopes = Object.fromEntries(
    SCOPES.map((scope) => [
      scope,
      Object.fromEntries(
        ACTIONS.map((action) => [action, matrix?.scopes?.[scope]?.[action] ?? 'none']),
      ),
    ]),
  ) as RoleFormValues['scopes']

  const fields = Object.fromEntries(
    FIELD_GROUPS.map((group) => [
      group,
      {
        read: matrix?.fields?.[group]?.read ?? false,
        edit: matrix?.fields?.[group]?.edit ?? false,
      },
    ]),
  ) as RoleFormValues['fields']

  return { name, scopes, fields }
}

export function toPermissionMatrix(values: RoleFormValues): PermissionMatrix {
  return { scopes: values.scopes, fields: values.fields }
}
