import { useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { toast } from 'sonner'
import { CheckboxList } from '@/components/CheckboxList'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { useAllMembers } from '@/features/settings/hooks/useAllMembers'
import { memberName } from '@/features/settings/memberName'
import { toastApiError } from '@/lib/api/errors'
import { getListTeamsQueryKey, useSetTeamMembers } from '@/lib/api/generated/endpoints'
import type { TeamDto } from '@/lib/api/generated/models'

export function TeamMembersDialog({ team, onClose }: { team: TeamDto; onClose: () => void }) {
  const queryClient = useQueryClient()
  const members = useAllMembers()
  const [membershipIds, setMembershipIds] = useState<string[]>(team.membershipIds)
  const setTeamMembers = useSetTeamMembers()

  const save = () =>
    setTeamMembers.mutate(
      { id: team.id, data: { membershipIds } },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListTeamsQueryKey() })
          toast.success('Team members updated.')
          onClose()
        },
        onError: toastApiError,
      },
    )

  const activeMembers = members.data?.items.filter((member) => member.status === 'active') ?? []

  return (
    <Dialog open onOpenChange={(open) => (open ? undefined : onClose())}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Members of '{team.name}'</DialogTitle>
          <DialogDescription>Choose who belongs to this team.</DialogDescription>
        </DialogHeader>
        {members.isPending ? <ListSkeleton rows={3} /> : null}
        {members.error ? (
          <ErrorState error={members.error} onRetry={() => void members.refetch()} />
        ) : null}
        {members.data ? (
          <CheckboxList
            idPrefix="team-member"
            options={activeMembers.map((member) => ({
              id: member.membershipId,
              label: memberName(member),
            }))}
            value={membershipIds}
            onChange={setMembershipIds}
          />
        ) : null}
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button disabled={!members.data || setTeamMembers.isPending} onClick={save}>
            Save members
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
