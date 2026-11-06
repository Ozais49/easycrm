import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { currentPrimary, isHexColor } from '@/lib/brand'

type BrandColorFieldProps = {
  /** A #RRGGBB color, or empty for the default. */
  value: string
  onChange: (value: string) => void
  error?: string
  disabled: boolean
}

export function BrandColorField({ value, onChange, error, disabled }: BrandColorFieldProps) {
  return (
    <FormField
      label="Brand color"
      htmlFor="brandColor"
      error={error}
      hint="Used for buttons and links. Leave empty for the default."
    >
      <div className="flex items-center gap-2">
        <input
          type="color"
          aria-label="Pick brand color"
          disabled={disabled}
          value={isHexColor(value) ? value : currentPrimary()}
          onChange={(event) => onChange(event.target.value)}
          className="border-input size-11 shrink-0 cursor-pointer rounded-md border bg-transparent p-1 disabled:cursor-not-allowed md:size-9"
        />
        <Input
          id="brandColor"
          placeholder="#RRGGBB"
          disabled={disabled}
          value={value}
          onChange={(event) => onChange(event.target.value)}
          aria-invalid={error ? true : undefined}
        />
        <Button
          type="button"
          variant="outline"
          disabled={disabled || value === ''}
          onClick={() => onChange('')}
        >
          Reset
        </Button>
      </div>
    </FormField>
  )
}
