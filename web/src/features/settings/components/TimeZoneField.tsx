import { useState } from 'react'
import { FormField } from '@/components/FormField'
import { Input } from '@/components/ui/input'
import { NativeSelect } from '@/components/ui/native-select'
import { COMMON_TIME_ZONES } from '@/features/settings/timeZones'

const OTHER = '__other__'

type TimeZoneFieldProps = {
  value: string
  onChange: (value: string) => void
  error?: string
  disabled: boolean
}

/** A select of common zones, plus free text for any other IANA id. */
export function TimeZoneField({ value, onChange, error, disabled }: TimeZoneFieldProps) {
  const [custom, setCustom] = useState(!COMMON_TIME_ZONES.includes(value))

  return (
    <FormField label="Time zone" htmlFor="timeZone" error={error}>
      <NativeSelect
        id="timeZone"
        disabled={disabled}
        value={custom ? OTHER : value}
        onChange={(event) => {
          if (event.target.value === OTHER) {
            setCustom(true)
          } else {
            setCustom(false)
            onChange(event.target.value)
          }
        }}
      >
        {COMMON_TIME_ZONES.map((zone) => (
          <option key={zone} value={zone}>
            {zone}
          </option>
        ))}
        <option value={OTHER}>Other…</option>
      </NativeSelect>
      {custom ? (
        <Input
          aria-label="Other time zone"
          placeholder="e.g. Pacific/Auckland"
          disabled={disabled}
          value={value}
          onChange={(event) => onChange(event.target.value)}
        />
      ) : null}
    </FormField>
  )
}
