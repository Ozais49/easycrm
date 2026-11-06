import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { Controller, useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { FormField } from '@/components/FormField'
import { PermissionGate } from '@/components/PermissionGate'
import { Button } from '@/components/ui/button'
import { NativeSelect } from '@/components/ui/native-select'
import { BrandColorField } from '@/features/settings/components/BrandColorField'
import { TimeZoneField } from '@/features/settings/components/TimeZoneField'
import { usePermission } from '@/features/identity/hooks/usePermission'
import {
  getGetTenantSettingsQueryKey,
  useUpdateTenantSettings,
} from '@/lib/api/generated/endpoints'
import type { TenantSettingsDto } from '@/lib/api/generated/models'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import { isHexColor } from '@/lib/brand'

const schema = z.object({
  timeZone: z.string().trim().min(1, 'Enter a time zone.'),
  dateDisplay: z.enum(['ad', 'bs', 'both']),
  brandColor: z
    .string()
    .trim()
    .refine((value) => value === '' || isHexColor(value), 'Use a color like #0D66D0.'),
})

type FormValues = z.infer<typeof schema>

const FIELDS = ['timeZone', 'dateDisplay', 'brandColor'] as const

const DATE_DISPLAY_LABELS: Record<FormValues['dateDisplay'], string> = {
  ad: 'AD (Gregorian)',
  bs: 'BS (Bikram Sambat)',
  both: 'Both AD and BS',
}

function toFormValues(settings: TenantSettingsDto): FormValues {
  return {
    timeZone: settings.timeZone,
    dateDisplay: settings.dateDisplay,
    brandColor: settings.brandColor ?? '',
  }
}

export function GeneralForm({ settings }: { settings: TenantSettingsDto }) {
  const queryClient = useQueryClient()
  const canEdit = usePermission('settings', 'edit')
  const update = useUpdateTenantSettings()
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: toFormValues(settings),
  })
  const { errors, isDirty } = form.formState

  const onSubmit = form.handleSubmit((values) =>
    update.mutate(
      {
        data: {
          timeZone: values.timeZone,
          dateDisplay: values.dateDisplay,
          brandColor: values.brandColor === '' ? null : values.brandColor,
        },
      },
      {
        onSuccess: (saved) => {
          queryClient.setQueryData(getGetTenantSettingsQueryKey(), saved)
          form.reset(toFormValues(saved))
          toast.success('Settings saved.')
        },
        onError: (error) => {
          if (!applyFieldErrors(error, form.setError, FIELDS)) toastApiError(error)
        },
      },
    ),
  )

  return (
    <form onSubmit={onSubmit} className="flex max-w-xl flex-col gap-5" noValidate>
      <Controller
        control={form.control}
        name="timeZone"
        render={({ field }) => (
          <TimeZoneField
            value={field.value}
            onChange={field.onChange}
            error={errors.timeZone?.message}
            disabled={!canEdit}
          />
        )}
      />

      <FormField label="Date display" htmlFor="dateDisplay" error={errors.dateDisplay?.message}>
        <NativeSelect id="dateDisplay" disabled={!canEdit} {...form.register('dateDisplay')}>
          {Object.entries(DATE_DISPLAY_LABELS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </NativeSelect>
      </FormField>

      <Controller
        control={form.control}
        name="brandColor"
        render={({ field }) => (
          <BrandColorField
            value={field.value}
            onChange={field.onChange}
            error={errors.brandColor?.message}
            disabled={!canEdit}
          />
        )}
      />

      <PermissionGate scope="settings" action="edit">
        <div>
          <Button type="submit" disabled={update.isPending || !isDirty}>
            Save changes
          </Button>
        </div>
      </PermissionGate>
    </form>
  )
}
