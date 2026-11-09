import { useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate, useParams } from '@tanstack/react-router'
import { useState } from 'react'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { PermissionGate } from '@/components/PermissionGate'
import { Button } from '@/components/ui/button'
import { formatAddress } from '@/features/crm/address'
import { DetailFields } from '@/features/crm/components/DetailFields'
import { contactName } from '@/features/crm/contactName'
import { useMemberDirectory, useTeamDirectory } from '@/features/crm/hooks/useDirectory'
import { toastApiError } from '@/lib/api/errors'
import {
  getListContactsQueryKey,
  useDeleteContact,
  useGetContact,
} from '@/lib/api/generated/endpoints'

export function ContactDetailPage() {
  const { contactId } = useParams({ from: '/contacts/$contactId' })
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const contact = useGetContact(contactId)
  const deleteContact = useDeleteContact()
  const members = useMemberDirectory()
  const teams = useTeamDirectory()
  const [confirmingDelete, setConfirmingDelete] = useState(false)

  const confirmDelete = () =>
    deleteContact.mutate(
      { id: contactId },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListContactsQueryKey() })
          toast.success('Contact deleted.')
          await navigate({ to: '/contacts' })
        },
        onError: (error) => {
          setConfirmingDelete(false)
          toastApiError(error)
        },
      },
    )

  if (contact.isPending) return <ListSkeleton rows={5} />
  if (contact.error) {
    return <ErrorState error={contact.error} onRetry={() => void contact.refetch()} />
  }

  const data = contact.data
  const ownerName = members.nameOf(data.ownerUserId) ?? (members.isSuccess ? 'Former member' : '—')
  const teamName = data.teamId
    ? (teams.nameOf(data.teamId) ?? (teams.isSuccess ? 'Removed team' : '—'))
    : '—'

  // Contact details may be absent from the response: undefined hides the field instead of showing "undefined".
  const address = data.address === undefined ? undefined : (formatAddress(data.address) ?? null)

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title={contactName(data)}
        description={data.jobTitle ?? undefined}
        action={
          <div className="flex flex-wrap gap-2">
            <PermissionGate scope="crm" action="edit">
              <Button variant="outline" asChild>
                <Link to="/contacts/$contactId/edit" params={{ contactId: data.id }}>
                  Edit
                </Link>
              </Button>
            </PermissionGate>
            <PermissionGate scope="crm" action="delete">
              <Button variant="outline" onClick={() => setConfirmingDelete(true)}>
                Delete
              </Button>
            </PermissionGate>
          </div>
        }
      />

      <DetailFields
        title="Details"
        fields={[
          { label: 'Job title', value: data.jobTitle },
          {
            label: 'Company',
            value:
              data.companyId && data.companyName ? (
                <Link
                  to="/companies/$companyId"
                  params={{ companyId: data.companyId }}
                  className="text-primary hover:underline"
                >
                  {data.companyName}
                </Link>
              ) : null,
          },
          { label: 'Email', value: data.email },
          { label: 'Phone', value: data.phone },
          { label: 'Mobile', value: data.mobile },
          { label: 'Address', value: address },
          { label: 'Owner', value: ownerName },
          { label: 'Team', value: teamName },
        ]}
      />

      <ConfirmDialog
        open={confirmingDelete}
        onOpenChange={setConfirmingDelete}
        title={`Delete contact '${contactName(data)}'?`}
        description="This can't be undone."
        confirmLabel="Delete contact"
        pending={deleteContact.isPending}
        onConfirm={confirmDelete}
      />
    </div>
  )
}
