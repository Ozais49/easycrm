import { useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { useAllTags } from '@/features/crm/hooks/useTags'
import { TagDialog } from '@/features/settings/components/TagDialog'
import { TAG_COLOR_LABELS } from '@/features/settings/tagColors'
import { toastApiError } from '@/lib/api/errors'
import { useDeleteTag } from '@/lib/api/generated/endpoints'
import type { TagDto } from '@/lib/api/generated/models'

const columns: Column<TagDto>[] = [
  { header: 'Tag', cell: (tag) => <Badge variant={tag.color}>{tag.name}</Badge> },
  { header: 'Color', cell: (tag) => TAG_COLOR_LABELS[tag.color] },
]

type ActiveDialog = { kind: 'edit'; tag: TagDto } | { kind: 'delete'; tag: TagDto }

export function TagsPage() {
  const queryClient = useQueryClient()
  const [dialog, setDialog] = useState<ActiveDialog | null>(null)
  const tags = useAllTags()
  const deleteTag = useDeleteTag()
  const close = () => setDialog(null)

  const confirmDelete = (tag: TagDto) =>
    deleteTag.mutate(
      { id: tag.id },
      {
        onSuccess: async () => {
          // The tag disappears from every company and contact, so refresh all lists.
          await queryClient.invalidateQueries()
          toast.success(`Tag '${tag.name}' deleted.`)
          close()
        },
        onError: (error) => {
          close()
          toastApiError(error)
        },
      },
    )

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Tags"
        description="Tags are created when you type them on a company or contact. Rename, recolor or delete them here."
      />
      {tags.isPending ? <ListSkeleton /> : null}
      {tags.error ? <ErrorState error={tags.error} onRetry={() => void tags.refetch()} /> : null}
      {tags.data && tags.data.length === 0 ? (
        <EmptyState message="No tags yet. Add one when editing a company or contact." />
      ) : null}
      {tags.data && tags.data.length > 0 ? (
        <ResponsiveTable
          items={tags.data}
          getKey={(tag) => tag.id}
          columns={columns}
          actions={(tag) => (
            <>
              <PermissionGate scope="settings" action="edit">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setDialog({ kind: 'edit', tag })}
                >
                  Edit
                </Button>
              </PermissionGate>
              <PermissionGate scope="settings" action="delete">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setDialog({ kind: 'delete', tag })}
                >
                  Delete
                </Button>
              </PermissionGate>
            </>
          )}
        />
      ) : null}

      {dialog?.kind === 'edit' ? <TagDialog tag={dialog.tag} onClose={close} /> : null}
      <ConfirmDialog
        open={dialog?.kind === 'delete'}
        onOpenChange={(open) => (open ? undefined : close())}
        title={`Delete tag '${dialog?.kind === 'delete' ? dialog.tag.name : ''}'?`}
        description="It is removed from every company and contact. This can't be undone."
        confirmLabel="Delete tag"
        pending={deleteTag.isPending}
        onConfirm={() => (dialog?.kind === 'delete' ? confirmDelete(dialog.tag) : undefined)}
      />
    </div>
  )
}
