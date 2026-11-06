import { parseChanges } from '@/features/settings/auditChanges'

export function AuditChanges({ changes }: { changes: unknown }) {
  const fields = parseChanges(changes)
  if (fields.length === 0) return <span className="text-muted-foreground">—</span>

  return (
    <ul className="flex flex-col gap-0.5">
      {fields.map(({ field, summary }) => (
        <li key={field} className="break-words">
          <span className="font-semibold">{field}</span>: {summary}
        </li>
      ))}
    </ul>
  )
}
