import { ClerkProvider } from '@clerk/react'
import { AuthGate } from '@/app/AuthGate'
import { clerkAppearance } from '@/app/clerkAppearance'
import { FullPageMessage } from '@/components/FullPageMessage'
import { clerkPublishableKey } from '@/lib/env'

export function App() {
  if (!clerkPublishableKey) {
    return (
      <FullPageMessage title="Sign-in isn't set up yet">
        Add VITE_CLERK_PUBLISHABLE_KEY to web/.env.local.
      </FullPageMessage>
    )
  }

  return (
    <ClerkProvider
      publishableKey={clerkPublishableKey}
      afterSignOutUrl="/"
      appearance={clerkAppearance}
    >
      <AuthGate />
    </ClerkProvider>
  )
}
