import type { UseFormReturn } from 'react-hook-form'
import { Label } from '@/components/ui/label'
import { NativeSelect } from '@/components/ui/native-select'
import { LEVELS, type RoleFormValues } from '@/features/settings/roleForm'
import { ACTION_LABELS, ACTIONS, LEVEL_LABELS, SCOPE_LABELS, SCOPES } from '@/lib/permissions'

const ROW_GRID = 'md:grid md:grid-cols-[minmax(0,1fr)_repeat(4,8rem)] md:items-center md:gap-3'

/** One row per scope on md+; one card per scope (actions in two columns) on mobile. */
export function ScopeMatrix({
  form,
  disabled,
}: {
  form: UseFormReturn<RoleFormValues>
  disabled: boolean
}) {
  return (
    <div className="flex flex-col gap-3">
      <div className={`${ROW_GRID} text-muted-foreground hidden px-3 text-sm font-semibold`}>
        <span>Area</span>
        {ACTIONS.map((action) => (
          <span key={action}>{ACTION_LABELS[action]}</span>
        ))}
      </div>
      {SCOPES.map((scope) => (
        <fieldset key={scope} className={`${ROW_GRID} rounded-lg border p-3`}>
          <legend className="sr-only">{SCOPE_LABELS[scope]}</legend>
          <p className="mb-3 font-semibold md:mb-0" aria-hidden="true">
            {SCOPE_LABELS[scope]}
          </p>
          <div className="grid grid-cols-2 gap-3 md:contents">
            {ACTIONS.map((action) => {
              const id = `scope-${scope}-${action}`
              return (
                <div key={action} className="flex flex-col gap-1.5">
                  <Label htmlFor={id} className="text-muted-foreground md:sr-only">
                    {ACTION_LABELS[action]}
                  </Label>
                  <NativeSelect
                    id={id}
                    disabled={disabled}
                    {...form.register(`scopes.${scope}.${action}`)}
                  >
                    {LEVELS.map((level) => (
                      <option key={level} value={level}>
                        {LEVEL_LABELS[level]}
                      </option>
                    ))}
                  </NativeSelect>
                </div>
              )
            })}
          </div>
        </fieldset>
      ))}
    </div>
  )
}
