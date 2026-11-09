import { Controller, useFormContext, type FieldErrors } from 'react-hook-form'
import { FormField } from '@/components/FormField'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { NativeSelect } from '@/components/ui/native-select'
import type { CustomFieldFormValues } from '@/features/crm/customFields'
import type { CustomFieldDto } from '@/lib/api/generated/models'

type FormShape = { customFields: CustomFieldFormValues }

/** The "Custom fields" section of a record form: one input per active definition, in display order. */
export function CustomFieldInputs({ definitions }: { definitions: readonly CustomFieldDto[] }) {
  const form = useFormContext<FormShape>()
  const errors = form.formState.errors as FieldErrors<{
    customFields: Record<string, string>
  }>

  if (definitions.length === 0) return null

  return (
    <section className="flex flex-col gap-4" aria-labelledby="custom-fields">
      <h2 id="custom-fields" className="text-xl font-semibold">
        Custom fields
      </h2>
      <div className="grid gap-4 sm:grid-cols-2">
        {definitions.map((definition) => {
          const name = `customFields.${definition.key}` as const
          const id = `custom-${definition.key}`
          const error = (errors.customFields as Record<string, { message?: string }> | undefined)?.[
            definition.key
          ]?.message
          const label = definition.required ? `${definition.label} (required)` : definition.label

          if (definition.type === 'checkbox') {
            return (
              <div key={definition.id} className="flex flex-col gap-1.5 sm:col-span-2">
                <Controller
                  control={form.control}
                  name={name}
                  render={({ field }) => (
                    <div className="flex items-center gap-2">
                      <Checkbox
                        id={id}
                        checked={field.value === true}
                        onCheckedChange={(checked) => field.onChange(checked === true)}
                        aria-invalid={error ? true : undefined}
                      />
                      <Label htmlFor={id}>{label}</Label>
                    </div>
                  )}
                />
                {error ? (
                  <p role="alert" className="text-destructive text-sm">
                    {error}
                  </p>
                ) : null}
              </div>
            )
          }

          return (
            <FormField key={definition.id} label={label} htmlFor={id} error={error}>
              {definition.type === 'select' ? (
                <NativeSelect
                  id={id}
                  aria-invalid={error ? true : undefined}
                  {...form.register(name)}
                >
                  <option value="">Not set</option>
                  {definition.options.map((option) => (
                    <option key={option} value={option}>
                      {option}
                    </option>
                  ))}
                </NativeSelect>
              ) : (
                <Input
                  id={id}
                  // Dates use the native picker (AD, yyyy-MM-dd) until the BS date picker exists.
                  type={definition.type === 'date' ? 'date' : 'text'}
                  inputMode={definition.type === 'number' ? 'decimal' : undefined}
                  aria-invalid={error ? true : undefined}
                  {...form.register(name)}
                />
              )}
            </FormField>
          )
        })}
      </div>
    </section>
  )
}
