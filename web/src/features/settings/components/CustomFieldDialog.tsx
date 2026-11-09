import { zodResolver } from '@hookform/resolvers/zod'
import { useQueryClient } from '@tanstack/react-query'
import { Plus, Trash2 } from 'lucide-react'
import { Controller, useFieldArray, useForm, useWatch } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { NativeSelect } from '@/components/ui/native-select'
import { CUSTOM_FIELD_TYPES, CUSTOM_FIELD_TYPE_LABELS } from '@/features/settings/customFieldTypes'
import { toastApiError } from '@/lib/api/errors'
import { applyFieldErrors } from '@/lib/api/formErrors'
import {
  getListCustomFieldsQueryKey,
  useCreateCustomField,
  useUpdateCustomField,
} from '@/lib/api/generated/endpoints'
import type { CustomFieldDto, CustomFieldEntity } from '@/lib/api/generated/models'

const MAX_OPTIONS = 50

const schema = z
  .object({
    label: z.string().trim().min(1, 'Enter a label.').max(80, 'Use 80 characters or fewer.'),
    type: z.enum(CUSTOM_FIELD_TYPES),
    required: z.boolean(),
    showInList: z.boolean(),
    options: z.array(z.object({ value: z.string().trim().max(80, 'Use 80 characters or fewer.') })),
  })
  .superRefine((values, context) => {
    if (values.type !== 'select') return
    const filled = values.options.map((option) => option.value).filter((value) => value !== '')
    if (filled.length === 0) {
      context.addIssue({ code: 'custom', path: ['options'], message: 'Add at least one option.' })
    }
    if (new Set(filled.map((value) => value.toLowerCase())).size !== filled.length) {
      context.addIssue({ code: 'custom', path: ['options'], message: 'Options must be unique.' })
    }
  })

type FormValues = z.infer<typeof schema>

type CustomFieldDialogProps = {
  entityType: CustomFieldEntity
  /** The field to edit; omit to create one. */
  field?: CustomFieldDto
  onClose: () => void
}

export function CustomFieldDialog({ entityType, field, onClose }: CustomFieldDialogProps) {
  const queryClient = useQueryClient()
  const create = useCreateCustomField()
  const update = useUpdateCustomField()
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      label: field?.label ?? '',
      type: field?.type ?? 'text',
      required: field?.required ?? false,
      showInList: field?.showInList ?? false,
      options: field?.options.map((value) => ({ value })) ?? [],
    },
  })
  const options = useFieldArray({ control: form.control, name: 'options' })
  const { errors } = form.formState
  const type = useWatch({ control: form.control, name: 'type' })
  const isSelect = type === 'select'
  const optionsError = errors.options?.message ?? errors.options?.root?.message

  const onSuccess = async () => {
    await queryClient.invalidateQueries({ queryKey: getListCustomFieldsQueryKey() })
    toast.success(field ? 'Custom field saved.' : 'Custom field created.')
    onClose()
  }
  const onError = (error: unknown) => {
    if (!applyFieldErrors(error, form.setError, ['label', 'options'])) toastApiError(error)
  }

  const onSubmit = form.handleSubmit((values) => {
    const data = {
      label: values.label,
      required: values.required,
      showInList: values.showInList,
      options: values.type === 'select' ? values.options.map((o) => o.value).filter(Boolean) : null,
    }
    if (field) update.mutate({ id: field.id, data }, { onSuccess, onError })
    else
      create.mutate(
        { data: { ...data, entityType, key: null, type: values.type } },
        { onSuccess, onError },
      )
  })

  return (
    <Dialog open onOpenChange={(open) => (open ? undefined : onClose())}>
      <DialogContent aria-describedby={undefined}>
        <DialogHeader>
          <DialogTitle>{field ? `Edit '${field.label}'` : 'New custom field'}</DialogTitle>
        </DialogHeader>
        <form onSubmit={onSubmit} className="flex flex-col gap-4" noValidate>
          <FormField label="Label" htmlFor="field-label" error={errors.label?.message}>
            <Input
              id="field-label"
              autoFocus
              aria-invalid={errors.label ? true : undefined}
              {...form.register('label')}
            />
          </FormField>
          <FormField
            label="Type"
            htmlFor="field-type"
            hint={field ? 'The type cannot be changed after the field is created.' : undefined}
          >
            <NativeSelect id="field-type" disabled={field !== undefined} {...form.register('type')}>
              {CUSTOM_FIELD_TYPES.map((value) => (
                <option key={value} value={value}>
                  {CUSTOM_FIELD_TYPE_LABELS[value]}
                </option>
              ))}
            </NativeSelect>
          </FormField>

          {isSelect ? (
            <fieldset className="flex flex-col gap-2">
              <legend className="text-sm font-medium">Options</legend>
              {options.fields.map((option, index) => (
                <div key={option.id} className="flex items-center gap-2">
                  <Input
                    aria-label={`Option ${index + 1}`}
                    {...form.register(`options.${index}.value`)}
                  />
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    aria-label={`Remove option ${index + 1}`}
                    onClick={() => options.remove(index)}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              ))}
              <div>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  disabled={options.fields.length >= MAX_OPTIONS}
                  onClick={() => options.append({ value: '' })}
                >
                  <Plus className="size-4" />
                  Add option
                </Button>
              </div>
              {optionsError ? (
                <p role="alert" className="text-destructive text-sm">
                  {optionsError}
                </p>
              ) : null}
            </fieldset>
          ) : null}

          <div className="flex flex-col gap-3">
            <Controller
              control={form.control}
              name="required"
              render={({ field: input }) => (
                <div className="flex items-center gap-2">
                  <Checkbox
                    id="field-required"
                    checked={input.value}
                    onCheckedChange={(checked) => input.onChange(checked === true)}
                  />
                  <Label htmlFor="field-required">Required</Label>
                </div>
              )}
            />
            <Controller
              control={form.control}
              name="showInList"
              render={({ field: input }) => (
                <div className="flex items-center gap-2">
                  <Checkbox
                    id="field-show-in-list"
                    checked={input.value}
                    onCheckedChange={(checked) => input.onChange(checked === true)}
                  />
                  <Label htmlFor="field-show-in-list">Show as a column in lists</Label>
                </div>
              )}
            />
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={create.isPending || update.isPending}>
              {field ? 'Save field' : 'Create field'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
