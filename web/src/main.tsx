import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import '@/styles/index.css'
import { App } from '@/app/App'
import { AppErrorBoundary } from '@/app/AppErrorBoundary'
import { ThemeProvider } from '@/app/theme'
import { Toaster } from '@/components/ui/sonner'

const root = document.getElementById('root')
if (!root) throw new Error('Missing #root element')

createRoot(root).render(
  <StrictMode>
    <ThemeProvider>
      <AppErrorBoundary>
        <App />
      </AppErrorBoundary>
      <Toaster />
    </ThemeProvider>
  </StrictMode>,
)
