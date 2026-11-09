import { Badge } from '@/components/ui/badge'
import type { TagDto } from '@/lib/api/generated/models'

/** A record's tags as colored chips; nothing when it has none. */
export function TagBadges({ tags }: { tags: readonly TagDto[] }) {
  if (tags.length === 0) return <span aria-label="No tags">—</span>
  return (
    <ul className="flex flex-wrap gap-1.5">
      {tags.map((tag) => (
        <li key={tag.id}>
          <Badge variant={tag.color}>{tag.name}</Badge>
        </li>
      ))}
    </ul>
  )
}
