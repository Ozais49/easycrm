import { Controller, type UseFormReturn } from 'react-hook-form'
import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'
import type { RoleFormValues } from '@/features/settings/roleForm'
import { FIELD_GROUP_LABELS, FIELD_GROUPS } from '@/lib/permissions'

const ACCESS = [
  { key: 'read', label: 'View' },
  { key: 'edit', label: 'Edit' },
] as const

export function FieldGroupMatrix({
  form,
  disabled,
}: {
  form: UseFormReturn<RoleFormValues>
  disabled: boolean
}) {
  return (
    <div className="flex flex-col gap-3">
      {FIELD_GROUPS.map((group) => (
        <fieldset
          key={group}
          className="flex flex-wrap items-center justify-between gap-x-6 gap-y-1 rounded-lg border p-3"
        >
          <legend className="sr-only">{FIELD_GROUP_LABELS[group]}</legend>
          <span className="font-semibold" aria-hidden="true">
            {FIELD_GROUP_LABELS[group]}
          </span>
          <div className="flex gap-2">
            {ACCESS.map(({ key, label }) => {
              const id = `field-${group}-${key}`
              return (
                <Controller
                  key={key}
                  control={form.control}
                  name={`fields.${group}.${key}`}
                  render={({ field }) => (
                    <Label
                      htmlFor={id}
                      className="flex min-h-11 items-center gap-2 px-2 font-normal"
                    >
                      <Checkbox
                        id={id}
                        disabled={disabled}
                        checked={field.value}
                        onCheckedChange={(checked) => field.onChange(checked === true)}
                      />
                      {label}
                    </Label>
                  )}
                />
              )
            })}
          </div>
        </fieldset>
      ))}
    </div>
  )
}
