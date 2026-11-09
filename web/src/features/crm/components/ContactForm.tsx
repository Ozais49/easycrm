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
import { CompanyPicker } from '@/features/crm/components/CompanyPicker'
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
import { useFieldPermission } from '@/features/identity/hooks/useFieldPermission'
import { useMe } from '@/features/identity/hooks/useMe'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import {
  getGetContactQueryKey,
  getListContactsQueryKey,
  useCreateContact,
  useUpdateContact,
} from '@/lib/api/generated/endpoints'
import type { ContactDto, CustomFieldDto } from '@/lib/api/generated/models'

const baseSchema = z.object({
  firstName: z
    .string()
    .trim()
    .min(1, 'Enter a first name.')
    .max(100, 'Use 100 characters or fewer.'),
  lastName: z.string().trim().max(100, 'Use 100 characters or fewer.'),
  jobTitle: z.string().trim().max(100, 'Use 100 characters or fewer.'),
  companyId: z.string(),
  email: optionalEmail,
  phone: z.string().trim().max(30, 'Use 30 characters or fewer.'),
  mobile: z.string().trim().max(30, 'Use 30 characters or fewer.'),
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
  'firstName',
  'lastName',
  'jobTitle',
  'companyId',
  'email',
  'phone',
  'mobile',
  'ownerUserId',
  'teamId',
] as const

function toFormValues(
  contact: ContactDto | undefined,
  meUserId: string,
  definitions: readonly CustomFieldDto[],
): FormValues {
  return {
    tags: contact?.tags.map((tag) => tag.name) ?? [],
    customFields: toCustomFieldFormValues(definitions, contact?.customFields),
    firstName: contact?.firstName ?? '',
    lastName: contact?.lastName ?? '',
    jobTitle: contact?.jobTitle ?? '',
    companyId: contact?.companyId ?? '',
    // Contact details are absent from the response for users without access to them.
    email: contact?.email ?? '',
    phone: contact?.phone ?? '',
    mobile: contact?.mobile ?? '',
    ...toAddressFormValues(contact?.address),
    // New records default to the creator; an existing unassigned record stays unassigned.
    ownerUserId: contact ? (contact.ownerUserId ?? '') : meUserId,
    teamId: contact?.teamId ?? '',
  }
}

type ContactFormProps = {
  contact?: ContactDto
  /** Pre-selects the company when creating from a company page. */
  companyId?: string
}

/** Creates a contact, or edits an existing one. The owner defaults to the current user. */
export function ContactForm({ contact, companyId }: ContactFormProps) {
  const me = useMe()
  const definitions = useCustomFieldDefinitions('contact')
  if (definitions.error) {
    return <ErrorState error={definitions.error} onRetry={() => void definitions.refetch()} />
  }
  if ((!contact && !me.data) || !definitions.data) return <ListSkeleton rows={5} />
  return (
    <ContactFormFields
      contact={contact}
      companyId={companyId}
      meUserId={me.data?.user.id ?? ''}
      definitions={definitions.data}
    />
  )
}

type ContactFormFieldsProps = ContactFormProps & {
  meUserId: string
  definitions: readonly CustomFieldDto[]
}

function ContactFormFields({ contact, companyId, meUserId, definitions }: ContactFormFieldsProps) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()
  const create = useCreateContact()
  const update = useUpdateContact()
  const pii = useFieldPermission('contactPii')
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
    defaultValues: {
      ...toFormValues(contact, meUserId, definitions),
      companyId: contact?.companyId ?? companyId ?? '',
    },
  })
  const { errors } = form.formState
  const pending = create.isPending || update.isPending

  const onSaved = async (saved: ContactDto) => {
    await queryClient.invalidateQueries({ queryKey: getListContactsQueryKey() })
    if (contact)
      await queryClient.invalidateQueries({ queryKey: getGetContactQueryKey(contact.id) })
    toast.success(contact ? 'Contact saved.' : 'Contact created.')
    await navigate({ to: '/contacts/$contactId', params: { contactId: saved.id } })
  }
  const onError = (error: unknown) => {
    const fields = [...FIELDS, ...customFieldPaths(definitions)] as Path<FormValues>[]
    if (!applyFieldErrors(error, form.setError, fields)) toastApiError(error)
  }

  const onSubmit = form.handleSubmit((values) => {
    // Without edit access to contact details the server rejects any value, so send none of them.
    const data = {
      firstName: values.firstName,
      lastName: orNull(values.lastName),
      jobTitle: orNull(values.jobTitle),
      companyId: orNull(values.companyId),
      email: pii.canEdit ? orNull(values.email) : null,
      phone: pii.canEdit ? orNull(values.phone) : null,
      mobile: pii.canEdit ? orNull(values.mobile) : null,
      address: pii.canEdit ? toAddressDto(values) : null,
      ownerUserId: orNull(values.ownerUserId),
      teamId: orNull(values.teamId),
      tags: values.tags,
      customFields: toCustomFieldsDto(definitions, values.customFields),
    }
    if (contact) update.mutate({ id: contact.id, data }, { onSuccess: onSaved, onError })
    else create.mutate({ data }, { onSuccess: onSaved, onError })
  })

  return (
    <FormProvider {...form}>
      <form onSubmit={onSubmit} className="flex max-w-3xl flex-col gap-8" noValidate>
        <section className="flex flex-col gap-4" aria-labelledby="contact-details">
          <h2 id="contact-details" className="text-xl font-semibold">
            Details
          </h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <FormField label="First name" htmlFor="firstName" error={errors.firstName?.message}>
              <Input
                id="firstName"
                autoFocus={!contact}
                aria-invalid={errors.firstName ? true : undefined}
                {...form.register('firstName')}
              />
            </FormField>
            <FormField label="Last name" htmlFor="lastName" error={errors.lastName?.message}>
              <Input id="lastName" {...form.register('lastName')} />
            </FormField>
            <FormField label="Job title" htmlFor="jobTitle" error={errors.jobTitle?.message}>
              <Input id="jobTitle" {...form.register('jobTitle')} />
            </FormField>
            <Controller
              control={form.control}
              name="companyId"
              render={({ field }) => (
                <CompanyPicker
                  value={field.value}
                  selectedName={contact?.companyName}
                  onChange={field.onChange}
                  error={errors.companyId?.message}
                />
              )}
            />
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

        {pii.canEdit ? (
          <>
            <section className="flex flex-col gap-4" aria-labelledby="contact-reach">
              <h2 id="contact-reach" className="text-xl font-semibold">
                Contact details
              </h2>
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="sm:col-span-2">
                  <FormField label="Email" htmlFor="email" error={errors.email?.message}>
                    <Input id="email" type="email" {...form.register('email')} />
                  </FormField>
                </div>
                <FormField label="Phone" htmlFor="phone" error={errors.phone?.message}>
                  <Input id="phone" type="tel" {...form.register('phone')} />
                </FormField>
                <FormField label="Mobile" htmlFor="mobile" error={errors.mobile?.message}>
                  <Input id="mobile" type="tel" {...form.register('mobile')} />
                </FormField>
              </div>
            </section>
            <section className="flex flex-col gap-4" aria-labelledby="contact-address">
              <h2 id="contact-address" className="text-xl font-semibold">
                Address
              </h2>
              <AddressFields />
            </section>
          </>
        ) : null}

        <section className="flex flex-col gap-4" aria-labelledby="contact-ownership">
          <h2 id="contact-ownership" className="text-xl font-semibold">
            Ownership
          </h2>
          <OwnerTeamFields />
        </section>

        <div className="flex flex-wrap gap-2">
          <Button type="submit" disabled={pending}>
            {contact ? 'Save changes' : 'Create contact'}
          </Button>
          <Button variant="outline" asChild>
            {contact ? (
              <Link to="/contacts/$contactId" params={{ contactId: contact.id }}>
                Cancel
              </Link>
            ) : (
              <Link to="/contacts">Cancel</Link>
            )}
          </Button>
        </div>
      </form>
    </FormProvider>
  )
}
