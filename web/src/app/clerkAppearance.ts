import type { ClerkProvider } from '@clerk/react'
import type { ComponentProps } from 'react'

type Appearance = NonNullable<ComponentProps<typeof ClerkProvider>['appearance']>

/** Points Clerk's components at our tokens so they follow the theme and the tenant brand color. */
export const clerkAppearance: Appearance = {
  variables: {
    colorPrimary: 'var(--primary)',
    colorBackground: 'var(--card)',
    colorForeground: 'var(--foreground)',
    colorMutedForeground: 'var(--muted-foreground)',
    colorInput: 'var(--background)',
    colorInputForeground: 'var(--foreground)',
    colorDanger: 'var(--destructive)',
    fontFamily: 'var(--font-sans)',
    borderRadius: '4px',
  },
}
