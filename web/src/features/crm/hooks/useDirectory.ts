import { useListDirectoryMembers, useListDirectoryTeams } from '@/lib/api/generated/endpoints'

const FIVE_MINUTES = 5 * 60_000

/** Owner picker options and a lookup from user id to name. Cached: the list changes rarely. */
export function useMemberDirectory() {
  const query = useListDirectoryMembers({ query: { staleTime: FIVE_MINUTES } })
  const nameOf = (userId: string) => query.data?.find((member) => member.userId === userId)?.name
  return { ...query, nameOf }
}

export function useTeamDirectory() {
  const query = useListDirectoryTeams({ query: { staleTime: FIVE_MINUTES } })
  const nameOf = (teamId: string) => query.data?.find((team) => team.id === teamId)?.name
  return { ...query, nameOf }
}
