import { useFormContext } from 'react-hook-form'
import { FormField } from '@/components/FormField'
import { Input } from '@/components/ui/input'
import { NativeSelect } from '@/components/ui/native-select'
import { PROVINCE_LABELS, PROVINCES, type AddressFormValues } from '@/features/crm/address'

/** Street, city, district and province inputs; render inside a FormProvider whose values include AddressFormValues. */
export function AddressFields() {
  const {
    register,
    formState: { errors },
  } = useFormContext<AddressFormValues>()

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <div className="sm:col-span-2">
        <FormField label="Street" htmlFor="street" error={errors.street?.message}>
          <Input id="street" autoComplete="street-address" {...register('street')} />
        </FormField>
      </div>
      <FormField label="City" htmlFor="city" error={errors.city?.message}>
        <Input id="city" {...register('city')} />
      </FormField>
      <FormField label="District" htmlFor="district" error={errors.district?.message}>
        <Input id="district" {...register('district')} />
      </FormField>
      <FormField label="Province" htmlFor="province" error={errors.province?.message}>
        <NativeSelect id="province" {...register('province')}>
          <option value="">Not set</option>
          {PROVINCES.map((province) => (
            <option key={province} value={province}>
              {PROVINCE_LABELS[province]}
            </option>
          ))}
        </NativeSelect>
      </FormField>
    </div>
  )
}
