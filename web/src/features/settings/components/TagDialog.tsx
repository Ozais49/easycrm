import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { useForm, useWatch } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { FormField } from '@/components/FormField'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { NativeSelect } from '@/components/ui/native-select'
import { TAG_COLORS, TAG_COLOR_LABELS } from '@/features/settings/tagColors'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import { useUpdateTag } from '@/lib/api/generated/endpoints'
import type { TagDto } from '@/lib/api/generated/models'

const schema = z.object({
  name: z.string().trim().min(1, 'Enter a tag name.').max(50, 'Use 50 characters or fewer.'),
  color: z.enum(TAG_COLORS),
})

type FormValues = z.infer<typeof schema>

export function TagDialog({ tag, onClose }: { tag: TagDto; onClose: () => void }) {
  const queryClient = useQueryClient()
  const update = useUpdateTag()
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: tag.name, color: tag.color },
  })
  const { errors } = form.formState
  const preview = useWatch({ control: form.control })

  const onSubmit = form.handleSubmit((values) =>
    update.mutate(
      { id: tag.id, data: values },
      {
        onSuccess: async () => {
          // Lists show tags on their rows, so refresh every query that can contain them.
          await queryClient.invalidateQueries()
          toast.success('Tag saved.')
          onClose()
        },
        onError: (error) => {
          if (!applyFieldErrors(error, form.setError, ['name', 'color'])) toastApiError(error)
        },
      },
    ),
  )

  return (
    <Dialog open onOpenChange={(open) => (open ? undefined : onClose())}>
      <DialogContent aria-describedby={undefined}>
        <DialogHeader>
          <DialogTitle>{`Edit tag '${tag.name}'`}</DialogTitle>
        </DialogHeader>
        <form onSubmit={onSubmit} className="flex flex-col gap-4" noValidate>
          <FormField label="Name" htmlFor="tag-name" error={errors.name?.message}>
            <Input
              id="tag-name"
              autoFocus
              aria-invalid={errors.name ? true : undefined}
              {...form.register('name')}
            />
          </FormField>
          <FormField label="Color" htmlFor="tag-color" error={errors.color?.message}>
            <NativeSelect id="tag-color" {...form.register('color')}>
              {TAG_COLORS.map((color) => (
                <option key={color} value={color}>
                  {TAG_COLOR_LABELS[color]}
                </option>
              ))}
            </NativeSelect>
          </FormField>
          <div className="flex items-center gap-2 text-sm">
            <span className="text-muted-foreground">Preview</span>
            <Badge variant={preview.color ?? 'neutral'}>
              {preview.name?.trim() ? preview.name : 'Tag'}
            </Badge>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={update.isPending}>
              Save tag
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
