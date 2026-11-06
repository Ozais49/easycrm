import { useEffect } from 'react'
import { useTenantSettings } from '@/features/settings/hooks/useTenantSettings'
import { applyBrandColor } from '@/lib/brand'

/** Applies the tenant brand color to the primary tokens. Renders nothing. */
export function BrandApplier() {
  const { data } = useTenantSettings()
  const brandColor = data?.brandColor

  useEffect(() => {
    applyBrandColor(brandColor)
    return () => applyBrandColor(null)
  }, [brandColor])

  return null
}
