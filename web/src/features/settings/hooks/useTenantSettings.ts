import { useGetTenantSettings } from '@/lib/api/generated/endpoints'

const FIVE_MINUTES = 5 * 60_000

export function useTenantSettings() {
  return useGetTenantSettings({ query: { staleTime: FIVE_MINUTES } })
}
