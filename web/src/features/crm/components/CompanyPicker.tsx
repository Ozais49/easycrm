import { useState } from 'react'
import { FormField } from '@/components/FormField'
import { Input } from '@/components/ui/input'
import { NativeSelect } from '@/components/ui/native-select'
import { useDebouncedValue } from '@/features/crm/hooks/useDebouncedValue'
import { useListCompanies } from '@/lib/api/generated/endpoints'

type CompanyPickerProps = {
  /** Selected company id; empty string means none. */
  value: string
  /** Name of the selected company, shown until the search results contain it. */
  selectedName?: string | null
  onChange: (companyId: string) => void
  error?: string
}

/** Search box plus a select of the matching companies (first 20); the current choice always stays listed. */
export function CompanyPicker({ value, selectedName, onChange, error }: CompanyPickerProps) {
  const [text, setText] = useState('')
  const q = useDebouncedValue(text.trim())
  const companies = useListCompanies({ q: q === '' ? undefined : q, pageSize: 20 })
  const items = companies.data?.items ?? []
  const selectedListed = items.some((company) => company.id === value)

  return (
    <FormField label="Company" htmlFor="companyId" error={error} hint="Type to search, then pick.">
      <div className="flex flex-col gap-2">
        <Input
          type="search"
          aria-label="Search companies"
          placeholder="Search companies"
          value={text}
          onChange={(event) => setText(event.target.value)}
        />
        <NativeSelect
          id="companyId"
          value={value}
          onChange={(event) => onChange(event.target.value)}
        >
          <option value="">No company</option>
          {value !== '' && !selectedListed ? (
            <option value={value}>{selectedName ?? 'Selected company'}</option>
          ) : null}
          {items.map((company) => (
            <option key={company.id} value={company.id}>
              {company.name}
            </option>
          ))}
        </NativeSelect>
      </div>
    </FormField>
  )
}
