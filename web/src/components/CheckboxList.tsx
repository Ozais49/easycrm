import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'

export type CheckboxOption = { id: string; label: string }

type CheckboxListProps = {
  options: readonly CheckboxOption[]
  value: readonly string[]
  onChange: (value: string[]) => void
  idPrefix: string
}

/** A multi-select: one big touch row per option. */
export function CheckboxList({ options, value, onChange, idPrefix }: CheckboxListProps) {
  const toggle = (id: string, checked: boolean) =>
    onChange(checked ? [...value, id] : value.filter((v) => v !== id))

  return (
    <ul className="flex max-h-72 flex-col overflow-y-auto rounded-md border">
      {options.map((option) => {
        const inputId = `${idPrefix}-${option.id}`
        return (
          <li key={option.id} className="border-b last:border-b-0">
            <Label
              htmlFor={inputId}
              className="flex min-h-11 cursor-pointer items-center gap-3 px-3 py-2 font-normal"
            >
              <Checkbox
                id={inputId}
                checked={value.includes(option.id)}
                onCheckedChange={(checked) => toggle(option.id, checked === true)}
              />
              <span className="break-words">{option.label}</span>
            </Label>
          </li>
        )
      })}
    </ul>
  )
}
