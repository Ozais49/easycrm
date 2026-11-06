import { useGetMe } from '@/lib/api/generated/endpoints'

const FIVE_MINUTES = 5 * 60_000

/** The signed-in user, their organization and effective permissions. Cached; the server re-checks everything. */
export function useMe() {
  return useGetMe({ query: { staleTime: FIVE_MINUTES } })
}
