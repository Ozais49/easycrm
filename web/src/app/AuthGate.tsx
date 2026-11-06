import { OrganizationList, SignIn, useAuth } from '@clerk/react'
import { useLayoutEffect, type ReactNode } from 'react'
import { OrgApp } from '@/app/OrgApp'
import { Skeleton } from '@/components/ui/skeleton'
import { registerTokenGetter } from '@/lib/api/client'

function Centered({ children }: { children: ReactNode }) {
  return <main className="flex min-h-dvh items-center justify-center p-4">{children}</main>
}

/** Signed out: sign-in. Signed in without an organization: pick or create one. Otherwise: the app. */
export function AuthGate() {
  const { isLoaded, isSignedIn, orgId, getToken } = useAuth()

  // Registered before any child effect can start a request.
  useLayoutEffect(() => {
    registerTokenGetter(() => getToken())
  }, [getToken])

  if (!isLoaded) {
    return (
      <Centered>
        <Skeleton className="h-10 w-48" />
      </Centered>
    )
  }

  if (!isSignedIn) {
    return (
      <Centered>
        <SignIn />
      </Centered>
    )
  }

  if (!orgId) {
    return (
      <Centered>
        <OrganizationList
          hidePersonal
          afterSelectOrganizationUrl="/"
          afterCreateOrganizationUrl="/"
        />
      </Centered>
    )
  }

  // Keyed by organization: switching starts a fresh query cache (see OrgApp).
  return <OrgApp key={orgId} />
}
