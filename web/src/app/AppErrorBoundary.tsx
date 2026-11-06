import { Component, type ErrorInfo, type ReactNode } from 'react'
import { FullPageMessage } from '@/components/FullPageMessage'
import { Button } from '@/components/ui/button'

type State = { failed: boolean }

/** Last line of defence: a friendly page instead of a blank screen. */
export class AppErrorBoundary extends Component<{ children: ReactNode }, State> {
  state: State = { failed: false }

  static getDerivedStateFromError(): State {
    return { failed: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error(error, info.componentStack)
  }

  render() {
    if (!this.state.failed) return this.props.children
    return (
      <FullPageMessage
        title="Something went wrong"
        action={<Button onClick={() => window.location.reload()}>Reload the page</Button>}
      >
        We hit an unexpected problem. Reload the page to try again.
      </FullPageMessage>
    )
  }
}
