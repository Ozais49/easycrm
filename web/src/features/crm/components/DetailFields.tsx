import type { ReactNode } from 'react'
import { Card } from '@/components/ui/card'

type DetailField = {
  label: string
  /** undefined hides the field (absent from the response or not permitted); null or empty shows a dash. */
  value: ReactNode | undefined
}

/** A panel of label/value pairs. */
export function DetailFields({ title, fields }: { title: string; fields: readonly DetailField[] }) {
  const visible = fields.filter((field) => field.value !== undefined)

  return (
    <Card>
      <h2 className="text-xl font-semibold">{title}</h2>
      <dl className="grid gap-4 sm:grid-cols-2">
        {visible.map((field) => (
          <div key={field.label} className="flex min-w-0 flex-col gap-0.5">
            <dt className="text-muted-foreground text-sm">{field.label}</dt>
            <dd className="break-words">
              {field.value === null || field.value === '' ? '—' : field.value}
            </dd>
          </div>
        ))}
      </dl>
    </Card>
  )
}
