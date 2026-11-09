import { X } from 'lucide-react'
import { useId, useState, type ChangeEvent, type KeyboardEvent } from 'react'
import { Badge } from '@/components/ui/badge'
import { Input } from '@/components/ui/input'
import type { TagDto } from '@/lib/api/generated/models'

export const MAX_TAGS = 20
export const MAX_TAG_LENGTH = 50
const MAX_SUGGESTIONS = 8

type TagInputProps = {
  id: string
  /** The tag names of the record. */
  value: readonly string[]
  onChange: (names: string[]) => void
  /** Existing tags of the organization; unknown names are created when the record is saved. */
  suggestions: readonly TagDto[]
  invalid?: boolean
}

const sameName = (a: string, b: string) => a.toLowerCase() === b.toLowerCase()

/**
 * Tags as chips. Enter or a comma adds the typed name, Backspace on an empty box removes the last chip,
 * and matching existing tags are offered as buttons below (works with touch and keyboard).
 */
export function TagInput({ id, value, onChange, suggestions, invalid }: TagInputProps) {
  const hintId = useId()
  const [text, setText] = useState('')
  const [focused, setFocused] = useState(false)
  const colorOf = new Map(suggestions.map((tag) => [tag.name.toLowerCase(), tag.color]))

  const addAll = (names: readonly string[]) => {
    const next = [...value]
    for (const raw of names) {
      const name = raw.trim().slice(0, MAX_TAG_LENGTH)
      if (
        name !== '' &&
        next.length < MAX_TAGS &&
        !next.some((existing) => sameName(existing, name))
      ) {
        next.push(name)
      }
    }
    if (next.length !== value.length) onChange(next)
  }

  const onTextChange = (event: ChangeEvent<HTMLInputElement>) => {
    const typed = event.target.value
    if (!typed.includes(',')) {
      setText(typed)
      return
    }
    // Phone keyboards do not always report the comma as a key press, so split on the typed text instead.
    const parts = typed.split(',')
    addAll(parts.slice(0, -1))
    setText(parts[parts.length - 1] ?? '')
  }

  const onKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter') {
      event.preventDefault() // Enter adds a tag; it must not submit the form.
      addAll([text])
      setText('')
    } else if (event.key === 'Backspace' && text === '' && value.length > 0) {
      onChange(value.slice(0, -1))
    }
  }

  const query = text.trim().toLowerCase()
  const matches = focused
    ? suggestions
        .filter(
          (tag) =>
            !value.some((name) => sameName(name, tag.name)) &&
            (query === '' || tag.name.toLowerCase().includes(query)),
        )
        .slice(0, MAX_SUGGESTIONS)
    : []

  return (
    <div className="flex flex-col gap-2">
      {value.length > 0 ? (
        <ul className="flex flex-wrap gap-1.5" aria-label="Selected tags">
          {value.map((name) => (
            <li key={name.toLowerCase()}>
              <Badge variant={colorOf.get(name.toLowerCase()) ?? 'neutral'} className="gap-1 pr-1">
                {name}
                <button
                  type="button"
                  aria-label={`Remove tag ${name}`}
                  className="flex size-6 items-center justify-center rounded-sm hover:bg-black/10 dark:hover:bg-white/10"
                  onClick={() => onChange(value.filter((existing) => existing !== name))}
                >
                  <X className="size-3.5" />
                </button>
              </Badge>
            </li>
          ))}
        </ul>
      ) : null}
      <Input
        id={id}
        value={text}
        maxLength={MAX_TAG_LENGTH + 1}
        autoComplete="off"
        enterKeyHint="done"
        placeholder={
          value.length >= MAX_TAGS ? 'Tag limit reached' : 'Type a tag, then press Enter'
        }
        disabled={value.length >= MAX_TAGS}
        aria-invalid={invalid ? true : undefined}
        aria-describedby={hintId}
        onChange={onTextChange}
        onKeyDown={onKeyDown}
        onFocus={() => setFocused(true)}
        onBlur={() => {
          setFocused(false)
          addAll([text])
          setText('')
        }}
      />
      <p id={hintId} className="text-muted-foreground text-sm">
        Separate tags with Enter or a comma. New tags are created when you save.
      </p>
      {matches.length > 0 ? (
        <ul className="flex flex-wrap gap-1.5" aria-label="Suggested tags">
          {matches.map((tag) => (
            <li key={tag.id}>
              <button
                type="button"
                className="border-input hover:bg-muted flex h-8 items-center rounded-md border px-2 text-sm"
                // Keep focus in the box so the suggestions do not vanish before the tap lands.
                onMouseDown={(event) => event.preventDefault()}
                onClick={() => {
                  addAll([tag.name])
                  setText('')
                }}
              >
                {tag.name}
              </button>
            </li>
          ))}
        </ul>
      ) : null}
    </div>
  )
}
