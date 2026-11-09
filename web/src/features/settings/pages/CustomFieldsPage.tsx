import { useQueryClient } from '@tanstack/react-query'
import { ArrowDown, ArrowUp } from 'lucide-react'
import { useState } from 'react'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Button } from '@/components/ui/button'
import { useCustomFieldDefinitions } from '@/features/crm/hooks/useCustomFieldDefinitions'
import { CustomFieldDialog } from '@/features/settings/components/CustomFieldDialog'
import { CUSTOM_FIELD_TYPE_LABELS } from '@/features/settings/customFieldTypes'
import { toastApiError } from '@/lib/api/errors'
import {
  getListCustomFieldsQueryKey,
  useDeleteCustomField,
  useReorderCustomFields,
} from '@/lib/api/generated/endpoints'
import type { CustomFieldDto, CustomFieldEntity } from '@/lib/api/generated/models'

const ENTITY_TABS: { value: CustomFieldEntity; label: string }[] = [
  { value: 'company', label: 'Companies' },
  { value: 'contact', label: 'Contacts' },
]

const columns: Column<CustomFieldDto>[] = [
  { header: 'Label', cell: (field) => field.label },
  { header: 'Type', cell: (field) => CUSTOM_FIELD_TYPE_LABELS[field.type] },
  { header: 'Required', cell: (field) => (field.required ? 'Yes' : 'No') },
  { header: 'In lists', cell: (field) => (field.showInList ? 'Yes' : 'No') },
  {
    header: 'Options',
    cell: (field) => (field.options.length > 0 ? field.options.join(', ') : '—'),
  },
]

type ActiveDialog =
  | { kind: 'create' }
  | { kind: 'edit'; field: CustomFieldDto }
  | { kind: 'delete'; field: CustomFieldDto }

export function CustomFieldsPage() {
  const queryClient = useQueryClient()
  const [entityType, setEntityType] = useState<CustomFieldEntity>('company')
  const [dialog, setDialog] = useState<ActiveDialog | null>(null)
  const fields = useCustomFieldDefinitions(entityType)
  const reorder = useReorderCustomFields()
  const deleteField = useDeleteCustomField()
  const close = () => setDialog(null)
  const refresh = () => queryClient.invalidateQueries({ queryKey: getListCustomFieldsQueryKey() })

  const move = (index: number, offset: -1 | 1) => {
    const ids = (fields.data ?? []).map((field) => field.id)
    const target = index + offset
    const moved = ids[index]
    const other = ids[target]
    if (moved === undefined || other === undefined) return
    ids[index] = other
    ids[target] = moved
    reorder.mutate(
      { data: { entityType, ids } },
      { onSuccess: () => void refresh(), onError: toastApiError },
    )
  }

  const confirmDelete = (field: CustomFieldDto) =>
    deleteField.mutate(
      { id: field.id },
      {
        onSuccess: async () => {
          await refresh()
          toast.success(`Custom field '${field.label}' deleted.`)
          close()
        },
        onError: (error) => {
          close()
          toastApiError(error)
        },
      },
    )

  const newFieldButton = (
    <PermissionGate scope="settings" action="edit">
      <Button onClick={() => setDialog({ kind: 'create' })}>New field</Button>
    </PermissionGate>
  )

  const items = fields.data ?? []

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Custom fields"
        description="Extra fields for companies and contacts. Deleting a field hides it; saved values are kept but no longer shown."
        action={newFieldButton}
      />
      <div role="group" aria-label="Record type" className="flex gap-2">
        {ENTITY_TABS.map((tab) => (
          <Button
            key={tab.value}
            variant={entityType === tab.value ? 'default' : 'outline'}
            aria-pressed={entityType === tab.value}
            onClick={() => setEntityType(tab.value)}
          >
            {tab.label}
          </Button>
        ))}
      </div>
      {fields.isPending ? <ListSkeleton /> : null}
      {fields.error ? (
        <ErrorState error={fields.error} onRetry={() => void fields.refetch()} />
      ) : null}
      {fields.data && items.length === 0 ? (
        <EmptyState message="No custom fields yet. Add the first one." action={newFieldButton} />
      ) : null}
      {items.length > 0 ? (
        <ResponsiveTable
          items={items}
          getKey={(field) => field.id}
          columns={columns}
          actions={(field) => {
            const index = items.indexOf(field)
            return (
              <>
                <PermissionGate scope="settings" action="edit">
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label={`Move ${field.label} up`}
                    disabled={index === 0 || reorder.isPending}
                    onClick={() => move(index, -1)}
                  >
                    <ArrowUp className="size-4" />
                  </Button>
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label={`Move ${field.label} down`}
                    disabled={index === items.length - 1 || reorder.isPending}
                    onClick={() => move(index, 1)}
                  >
                    <ArrowDown className="size-4" />
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDialog({ kind: 'edit', field })}
                  >
                    Edit
                  </Button>
                </PermissionGate>
                <PermissionGate scope="settings" action="delete">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDialog({ kind: 'delete', field })}
                  >
                    Delete
                  </Button>
                </PermissionGate>
              </>
            )
          }}
        />
      ) : null}

      {dialog?.kind === 'create' ? (
        <CustomFieldDialog entityType={entityType} onClose={close} />
      ) : null}
      {dialog?.kind === 'edit' ? (
        <CustomFieldDialog entityType={entityType} field={dialog.field} onClose={close} />
      ) : null}
      <ConfirmDialog
        open={dialog?.kind === 'delete'}
        onOpenChange={(open) => (open ? undefined : close())}
        title={`Delete custom field '${dialog?.kind === 'delete' ? dialog.field.label : ''}'?`}
        description="The field disappears from forms and lists. Values already saved on records are kept but hidden."
        confirmLabel="Delete field"
        pending={deleteField.isPending}
        onConfirm={() => (dialog?.kind === 'delete' ? confirmDelete(dialog.field) : undefined)}
      />
    </div>
  )
}
