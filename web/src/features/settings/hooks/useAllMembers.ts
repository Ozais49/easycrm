import { useListMembers } from '@/lib/api/generated/endpoints'

/** Every member of the organization (the API caps a page at 100). */
export function useAllMembers() {
  return useListMembers({ pageSize: 100 })
}
