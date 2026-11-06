import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import { getListTeamsQueryKey, useCreateTeam, useRenameTeam } from '@/lib/api/generated/endpoints'
import type { TeamDto } from '@/lib/api/generated/models'

const schema = z.object({
  name: z.string().trim().min(1, 'Enter a team name.').max(100, 'Use 100 characters or fewer.'),
})

type FormValues = z.infer<typeof schema>

type TeamNameDialogProps = {
  /** The team to rename; omit to create one. */
  team?: TeamDto
  onClose: () => void
}

export function TeamNameDialog({ team, onClose }: TeamNameDialogProps) {
  const queryClient = useQueryClient()
  const create = useCreateTeam()
  const rename = useRenameTeam()
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: team?.name ?? '' },
  })

  const onSuccess = async () => {
    await queryClient.invalidateQueries({ queryKey: getListTeamsQueryKey() })
    toast.success(team ? 'Team renamed.' : 'Team created.')
    onClose()
  }
  const onError = (error: unknown) => {
    if (!applyFieldErrors(error, form.setError, ['name'])) toastApiError(error)
  }

  const onSubmit = form.handleSubmit(({ name }) => {
    if (team) rename.mutate({ id: team.id, data: { name } }, { onSuccess, onError })
    else create.mutate({ data: { name } }, { onSuccess, onError })
  })

  return (
    <Dialog open onOpenChange={(open) => (open ? undefined : onClose())}>
      <DialogContent aria-describedby={undefined}>
        <DialogHeader>
          <DialogTitle>{team ? `Rename team '${team.name}'` : 'New team'}</DialogTitle>
        </DialogHeader>
        <form onSubmit={onSubmit} className="flex flex-col gap-4" noValidate>
          <FormField
            label="Team name"
            htmlFor="team-name"
            error={form.formState.errors.name?.message}
          >
            <Input
              id="team-name"
              autoFocus
              aria-invalid={form.formState.errors.name ? true : undefined}
              {...form.register('name')}
            />
          </FormField>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={create.isPending || rename.isPending}>
              {team ? 'Save name' : 'Create team'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
