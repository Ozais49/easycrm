import type { ClerkProvider } from '@clerk/react'
import type { ComponentProps } from 'react'

type Appearance = NonNullable<ComponentProps<typeof ClerkProvider>['appearance']>

/** Points Clerk's components at our tokens so they follow the theme and the tenant brand color. */
export const clerkAppearance: Appearance = {
  variables: {
    colorPrimary: 'var(--primary)',
    colorPrimaryForeground: 'var(--primary-foreground)',
    colorBackground: 'var(--card)',
    colorForeground: 'var(--foreground)',
    // Clerk's neutral (borders, hover fills, secondary text) defaults to black, unreadable in dark mode.
    colorNeutral: 'var(--foreground)',
    colorMuted: 'var(--muted)',
    colorMutedForeground: 'var(--muted-foreground)',
    colorBorder: 'var(--border)',
    colorRing: 'var(--ring)',
    colorInput: 'var(--background)',
    colorInputForeground: 'var(--foreground)',
    colorDanger: 'var(--destructive)',
    fontFamily: 'var(--font-sans)',
    borderRadius: '4px',
  },
}
