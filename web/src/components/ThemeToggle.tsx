import { Monitor, Moon, Sun } from 'lucide-react'
import { useTheme, type ThemePreference } from '@/app/theme'
import { Button } from '@/components/ui/button'

const NEXT: Record<ThemePreference, ThemePreference> = {
  light: 'dark',
  dark: 'system',
  system: 'light',
}

const LABELS: Record<ThemePreference, string> = {
  light: 'Light theme',
  dark: 'Dark theme',
  system: 'System theme',
}

const ICONS = { light: Sun, dark: Moon, system: Monitor }

/** Cycles light, dark, system. */
export function ThemeToggle() {
  const { preference, setPreference } = useTheme()
  const Icon = ICONS[preference]
  return (
    <Button
      variant="ghost"
      size="icon"
      title={`${LABELS[preference]} (select to change)`}
      aria-label={`${LABELS[preference]}. Select to change.`}
      onClick={() => setPreference(NEXT[preference])}
    >
      <Icon />
    </Button>
  )
}
