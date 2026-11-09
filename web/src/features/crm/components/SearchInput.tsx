import { Search } from 'lucide-react'
import { useEffect, useState } from 'react'
import { Input } from '@/components/ui/input'
import { useDebouncedValue } from '@/features/crm/hooks/useDebouncedValue'

type SearchInputProps = {
  label: string
  placeholder: string
  /** Called with the trimmed text once typing pauses. Must be stable (useCallback or a state setter). */
  onSearch: (text: string) => void
}

export function SearchInput({ label, placeholder, onSearch }: SearchInputProps) {
  const [text, setText] = useState('')
  const debounced = useDebouncedValue(text.trim())

  useEffect(() => {
    onSearch(debounced)
  }, [debounced, onSearch])

  return (
    <div className="relative w-full sm:max-w-sm">
      <Search className="text-muted-foreground pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2" />
      <Input
        type="search"
        aria-label={label}
        placeholder={placeholder}
        value={text}
        onChange={(event) => setText(event.target.value)}
        className="pl-9"
      />
    </div>
  )
}
