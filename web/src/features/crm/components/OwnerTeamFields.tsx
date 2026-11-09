import { useFormContext } from 'react-hook-form'
import { FormField } from '@/components/FormField'
import { NativeSelect } from '@/components/ui/native-select'
import { useMemberDirectory, useTeamDirectory } from '@/features/crm/hooks/useDirectory'

export type OwnerTeamFormValues = {
  /** Empty string means unassigned. */
  ownerUserId: string
  /** Empty string means no team. */
  teamId: string
}

/** Owner and optional team pickers fed by the directory; render inside a FormProvider with OwnerTeamFormValues. */
export function OwnerTeamFields() {
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext<OwnerTeamFormValues>()
  const members = useMemberDirectory()
  const teams = useTeamDirectory()
  const ownerUserId = watch('ownerUserId')
  const teamId = watch('teamId')

  // A record may point at someone who left or a team that was deleted: keep it selectable instead of blanking it.
  const ownerMissing =
    members.data !== undefined &&
    ownerUserId !== '' &&
    !members.data.some((member) => member.userId === ownerUserId)
  const teamMissing =
    teams.data !== undefined && teamId !== '' && !teams.data.some((team) => team.id === teamId)

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <FormField label="Owner" htmlFor="ownerUserId" error={errors.ownerUserId?.message}>
        <NativeSelect id="ownerUserId" {...register('ownerUserId')}>
          <option value="">Unassigned</option>
          {ownerMissing ? <option value={ownerUserId}>Former member</option> : null}
          {members.data?.map((member) => (
            <option key={member.userId} value={member.userId}>
              {member.name}
            </option>
          ))}
        </NativeSelect>
      </FormField>
      <FormField label="Team" htmlFor="teamId" error={errors.teamId?.message}>
        <NativeSelect id="teamId" {...register('teamId')}>
          <option value="">No team</option>
          {teamMissing ? <option value={teamId}>Removed team</option> : null}
          {teams.data?.map((team) => (
            <option key={team.id} value={team.id}>
              {team.name}
            </option>
          ))}
        </NativeSelect>
      </FormField>
    </div>
  )
}
