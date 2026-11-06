import { useListRoles } from '@/lib/api/generated/endpoints'

/** Every role of the organization (the API caps a page at 100; tenants have a handful). */
export function useAllRoles() {
  return useListRoles({ pageSize: 100 })
}
