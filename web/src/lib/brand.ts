const HEX = /^#[0-9a-f]{6}$/i

function channel(value: number): number {
  const c = value / 255
  return c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4
}

function luminance(hex: string): number {
  const n = parseInt(hex.slice(1), 16)
  return (
    0.2126 * channel((n >> 16) & 255) + 0.7152 * channel((n >> 8) & 255) + 0.0722 * channel(n & 255)
  )
}

export function isHexColor(value: string): boolean {
  return HEX.test(value)
}

/** Black or white, whichever reads better on the given #RRGGBB background (WCAG contrast). */
export function readableForeground(background: string): '#000000' | '#ffffff' {
  const l = luminance(background)
  const whiteContrast = 1.05 / (l + 0.05)
  const blackContrast = (l + 0.05) / 0.05
  return whiteContrast >= blackContrast ? '#ffffff' : '#000000'
}

/** Overrides the primary tokens for both themes; a missing or invalid color restores the defaults. */
export function applyBrandColor(color: string | null | undefined) {
  const style = document.documentElement.style
  if (color && isHexColor(color)) {
    style.setProperty('--primary', color)
    style.setProperty('--primary-foreground', readableForeground(color))
  } else {
    style.removeProperty('--primary')
    style.removeProperty('--primary-foreground')
  }
}

/** The primary color currently in effect (the default token or the tenant override). */
export function currentPrimary(): string {
  return getComputedStyle(document.documentElement).getPropertyValue('--primary').trim()
}
