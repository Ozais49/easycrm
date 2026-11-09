import { NativeSelect } from '@/components/ui/native-select'
import { useAllTags } from '@/features/crm/hooks/useTags'

type TagFilterProps = {
  /** The selected tag id; empty for all. */
  value: string
  onChange: (tagId: string) => void
}

/** Narrows a list to one tag. Hidden while the organization has no tags. */
export function TagFilter({ value, onChange }: TagFilterProps) {
  const tags = useAllTags()
  if (!tags.data || tags.data.length === 0) return null

  return (
    <div className="w-full sm:max-w-48">
      <NativeSelect
        aria-label="Filter by tag"
        value={value}
        onChange={(event) => onChange(event.target.value)}
      >
        <option value="">All tags</option>
        {tags.data.map((tag) => (
          <option key={tag.id} value={tag.id}>
            {tag.name}
          </option>
        ))}
      </NativeSelect>
    </div>
  )
}
