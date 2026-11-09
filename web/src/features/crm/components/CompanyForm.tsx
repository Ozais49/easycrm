import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate } from '@tanstack/react-router'
import { useMemo } from 'react'
import { Controller, FormProvider, useForm, type Path, type Resolver } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { ErrorState } from '@/components/ErrorState'
import { FormField } from '@/components/FormField'
import { ListSkeleton } from '@/components/ListSkeleton'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  addressShape,
  optionalEmail,
  orNull,
  toAddressDto,
  toAddressFormValues,
} from '@/features/crm/address'
import { AddressFields } from '@/features/crm/components/AddressFields'
import { CustomFieldInputs } from '@/features/crm/components/CustomFieldInputs'
import { OwnerTeamFields } from '@/features/crm/components/OwnerTeamFields'
import { TagInput } from '@/features/crm/components/TagInput'
import {
  customFieldPaths,
  customFieldsSchema,
  toCustomFieldFormValues,
  toCustomFieldsDto,
  type CustomFieldFormValues,
} from '@/features/crm/customFields'
import { useCustomFieldDefinitions } from '@/features/crm/hooks/useCustomFieldDefinitions'
import { useAllTags } from '@/features/crm/hooks/useTags'
import { useMe } from '@/features/identity/hooks/useMe'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import {
  getGetCompanyQueryKey,
  getListCompaniesQueryKey,
  useCreateCompany,
  useUpdateCompany,
} from '@/lib/api/generated/endpoints'
import type { CompanyDto, CustomFieldDto } from '@/lib/api/generated/models'

const baseSchema = z.object({
  name: z.string().trim().min(1, 'Enter a name.').max(200, 'Use 200 characters or fewer.'),
  industry: z.string().trim().max(100, 'Use 100 characters or fewer.'),
  website: z.string().trim().max(300, 'Use 300 characters or fewer.'),
  phone: z.string().trim().max(30, 'Use 30 characters or fewer.'),
  email: optionalEmail,
  panNumber: z
    .string()
    .trim()
    .refine((value) => value === '' || /^\d{9}$/.test(value), 'PAN must be exactly 9 digits.'),
  ...addressShape,
  // Empty string = unassigned.
  ownerUserId: z.string(),
  teamId: z.string(),
})

type FormValues = z.infer<typeof baseSchema> & {
  tags: string[]
  customFields: CustomFieldFormValues
}

const FIELDS = [
  'tags',
  'name',
  'industry',
  'website',
  'phone',
  'email',
  'panNumber',
  'ownerUserId',
  'teamId',
] as const

function toFormValues(
  company: CompanyDto | undefined,
  meUserId: string,
  definitions: readonly CustomFieldDto[],
): FormValues {
  return {
    tags: company?.tags.map((tag) => tag.name) ?? [],
    customFields: toCustomFieldFormValues(definitions, company?.customFields),
    name: company?.name ?? '',
    industry: company?.industry ?? '',
    website: company?.website ?? '',
    phone: company?.phone ?? '',
    email: company?.email ?? '',
    panNumber: company?.panNumber ?? '',
    ...toAddressFormValues(company?.address),
    // New records default to the creator; an existing unassigned record stays unassigned.
    ownerUserId: company ? (company.ownerUserId ?? '') : meUserId,
    teamId: company?.teamId ?? '',
  }
}

/** Creates a company, or edits an existing one. The owner defaults to the current user. */
export function CompanyForm({ company }: { company?: CompanyDto }) {
  const me = useMe()
  const definitions = useCustomFieldDefinitions('company')
  if (definitions.error) {
    return <ErrorState error={definitions.error} onRetry={() => void definitions.refetch()} />
  }
  if ((!company && !me.data) || !definitions.data) return <ListSkeleton rows={5} />
  return (
    <CompanyFormFields
      company={company}
      meUserId={me.data?.user.id ?? ''}
      definitions={definitions.data}
    />
  )
}

type CompanyFormFieldsProps = {
  company?: CompanyDto
  meUserId: string
  definitions: readonly CustomFieldDto[]
}

function CompanyFormFields({ company, meUserId, definitions }: CompanyFormFieldsProps) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()
  const create = useCreateCompany()
  const update = useUpdateCompany()
  const tags = useAllTags()
  const schema = useMemo(
    () =>
      baseSchema.extend({
        tags: z.array(z.string()),
        customFields: customFieldsSchema(definitions),
      }),
    [definitions],
  )
  const form = useForm<FormValues>({
    resolver: zodResolver(schema) as unknown as Resolver<FormValues>,
    defaultValues: toFormValues(company, meUserId, definitions),
  })
  const { errors } = form.formState
  const pending = create.isPending || update.isPending

  const onSaved = async (saved: CompanyDto) => {
    await queryClient.invalidateQueries({ queryKey: getListCompaniesQueryKey() })
    if (company)
      await queryClient.invalidateQueries({ queryKey: getGetCompanyQueryKey(company.id) })
    toast.success(company ? 'Company saved.' : 'Company created.')
    await navigate({ to: '/companies/$companyId', params: { companyId: saved.id } })
  }
  const onError = (error: unknown) => {
    const fields = [...FIELDS, ...customFieldPaths(definitions)] as Path<FormValues>[]
    if (!applyFieldErrors(error, form.setError, fields)) toastApiError(error)
  }

  const onSubmit = form.handleSubmit((values) => {
    const data = {
      name: values.name,
      industry: orNull(values.industry),
      website: orNull(values.website),
      phone: orNull(values.phone),
      email: orNull(values.email),
      panNumber: orNull(values.panNumber),
      address: toAddressDto(values),
      ownerUserId: orNull(values.ownerUserId),
      teamId: orNull(values.teamId),
      tags: values.tags,
      customFields: toCustomFieldsDto(definitions, values.customFields),
    }
    if (company) update.mutate({ id: company.id, data }, { onSuccess: onSaved, onError })
    else create.mutate({ data }, { onSuccess: onSaved, onError })
  })

  return (
    <FormProvider {...form}>
      <form onSubmit={onSubmit} className="flex max-w-3xl flex-col gap-8" noValidate>
        <section className="flex flex-col gap-4" aria-labelledby="company-details">
          <h2 id="company-details" className="text-xl font-semibold">
            Details
          </h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <FormField label="Name" htmlFor="name" error={errors.name?.message}>
                <Input
                  id="name"
                  autoFocus={!company}
                  aria-invalid={errors.name ? true : undefined}
                  {...form.register('name')}
                />
              </FormField>
            </div>
            <FormField label="Industry" htmlFor="industry" error={errors.industry?.message}>
              <Input id="industry" {...form.register('industry')} />
            </FormField>
            <FormField label="Website" htmlFor="website" error={errors.website?.message}>
              <Input id="website" type="url" inputMode="url" {...form.register('website')} />
            </FormField>
            <FormField label="Phone" htmlFor="phone" error={errors.phone?.message}>
              <Input id="phone" type="tel" {...form.register('phone')} />
            </FormField>
            <FormField label="Email" htmlFor="email" error={errors.email?.message}>
              <Input id="email" type="email" {...form.register('email')} />
            </FormField>
            <FormField
              label="PAN number"
              htmlFor="panNumber"
              error={errors.panNumber?.message}
              hint="Nine digits."
            >
              <Input id="panNumber" inputMode="numeric" {...form.register('panNumber')} />
            </FormField>
            <div className="sm:col-span-2">
              <FormField label="Tags" htmlFor="tags" error={errors.tags?.message}>
                <Controller
                  control={form.control}
                  name="tags"
                  render={({ field }) => (
                    <TagInput
                      id="tags"
                      value={field.value}
                      onChange={field.onChange}
                      suggestions={tags.data ?? []}
                      invalid={Boolean(errors.tags)}
                    />
                  )}
                />
              </FormField>
            </div>
          </div>
        </section>

        <CustomFieldInputs definitions={definitions} />

        <section className="flex flex-col gap-4" aria-labelledby="company-address">
          <h2 id="company-address" className="text-xl font-semibold">
            Address
          </h2>
          <AddressFields />
        </section>

        <section className="flex flex-col gap-4" aria-labelledby="company-ownership">
          <h2 id="company-ownership" className="text-xl font-semibold">
            Ownership
          </h2>
          <OwnerTeamFields />
        </section>

        <div className="flex flex-wrap gap-2">
          <Button type="submit" disabled={pending}>
            {company ? 'Save changes' : 'Create company'}
          </Button>
          <Button variant="outline" asChild>
            {company ? (
              <Link to="/companies/$companyId" params={{ companyId: company.id }}>
                Cancel
              </Link>
            ) : (
              <Link to="/companies">Cancel</Link>
            )}
          </Button>
        </div>
      </form>
    </FormProvider>
  )
}
