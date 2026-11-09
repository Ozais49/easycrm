import { useListTags } from '@/lib/api/generated/endpoints'

/** Every tag of the organization (the API caps the list at 500), for suggestions and filters. */
export function useAllTags() {
  return useListTags()
}
