import { useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { toast } from 'sonner'
import { CheckboxList } from '@/components/CheckboxList'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { memberName } from '@/features/settings/memberName'
import { toastApiError } from '@/lib/api/errors'
import { getListMembersQueryKey, useSetMemberRoles } from '@/lib/api/generated/endpoints'
import type { MemberDto, RoleDto } from '@/lib/api/generated/models'

type MemberRolesDialogProps = {
  member: MemberDto
  roles: readonly RoleDto[]
  onClose: () => void
}

export function MemberRolesDialog({ member, roles, onClose }: MemberRolesDialogProps) {
  const queryClient = useQueryClient()
  const [roleIds, setRoleIds] = useState<string[]>(member.roleIds)
  const setRoles = useSetMemberRoles()

  const save = () =>
    setRoles.mutate(
      { membershipId: member.membershipId, data: { roleIds } },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListMembersQueryKey() })
          toast.success('Roles updated.')
          onClose()
        },
        onError: toastApiError,
      },
    )

  return (
    <Dialog open onOpenChange={(open) => (open ? undefined : onClose())}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Roles for {memberName(member)}</DialogTitle>
          <DialogDescription>
            Pick at least one role. Permissions add up across roles.
          </DialogDescription>
        </DialogHeader>
        <CheckboxList
          idPrefix="member-role"
          options={roles.map((role) => ({ id: role.id, label: role.name }))}
          value={roleIds}
          onChange={setRoleIds}
        />
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button disabled={roleIds.length === 0 || setRoles.isPending} onClick={save}>
            Save roles
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
