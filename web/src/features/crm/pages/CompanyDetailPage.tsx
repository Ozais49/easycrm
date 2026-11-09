import { useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate, useParams } from '@tanstack/react-router'
import { useState } from 'react'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { Pagination } from '@/components/Pagination'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { formatAddress } from '@/features/crm/address'
import { DetailFields } from '@/features/crm/components/DetailFields'
import { useMemberDirectory, useTeamDirectory } from '@/features/crm/hooks/useDirectory'
import { useFieldPermission } from '@/features/identity/hooks/useFieldPermission'
import { toastApiError } from '@/lib/api/errors'
import {
  getListCompaniesQueryKey,
  useDeleteCompany,
  useGetCompany,
  useListContacts,
} from '@/lib/api/generated/endpoints'
import type { ContactDto } from '@/lib/api/generated/models'

const CONTACTS_PAGE_SIZE = 10

export function CompanyDetailPage() {
  const { companyId } = useParams({ from: '/companies/$companyId' })
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const company = useGetCompany(companyId)
  const deleteCompany = useDeleteCompany()
  const members = useMemberDirectory()
  const teams = useTeamDirectory()
  const [confirmingDelete, setConfirmingDelete] = useState(false)

  const confirmDelete = () =>
    deleteCompany.mutate(
      { id: companyId },
      {
        onSuccess: async () => {
          await queryClient.invalidateQueries({ queryKey: getListCompaniesQueryKey() })
          toast.success('Company deleted.')
          await navigate({ to: '/companies' })
        },
        onError: (error) => {
          setConfirmingDelete(false)
          toastApiError(error)
        },
      },
    )

  if (company.isPending) return <ListSkeleton rows={5} />
  if (company.error) {
    return <ErrorState error={company.error} onRetry={() => void company.refetch()} />
  }

  const data = company.data
  const ownerName = members.nameOf(data.ownerUserId) ?? (members.isSuccess ? 'Former member' : '—')
  const teamName = data.teamId
    ? (teams.nameOf(data.teamId) ?? (teams.isSuccess ? 'Removed team' : '—'))
    : '—'

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title={data.name}
        description={data.industry ?? undefined}
        action={
          <div className="flex flex-wrap gap-2">
            <PermissionGate scope="crm" action="edit">
              <Button variant="outline" asChild>
                <Link to="/companies/$companyId/edit" params={{ companyId: data.id }}>
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
          { label: 'Industry', value: data.industry },
          { label: 'Website', value: data.website },
          { label: 'Phone', value: data.phone },
          { label: 'Email', value: data.email },
          { label: 'PAN number', value: data.panNumber },
          { label: 'Address', value: formatAddress(data.address) ?? null },
          { label: 'Owner', value: ownerName },
          { label: 'Team', value: teamName },
        ]}
      />

      <CompanyContacts companyId={data.id} />

      <ConfirmDialog
        open={confirmingDelete}
        onOpenChange={setConfirmingDelete}
        title={`Delete company '${data.name}'?`}
        description="Its contacts stay, but are no longer linked to a company. This can't be undone."
        confirmLabel="Delete company"
        pending={deleteCompany.isPending}
        onConfirm={confirmDelete}
      />
    </div>
  )
}

function CompanyContacts({ companyId }: { companyId: string }) {
  const [page, setPage] = useState(1)
  const contacts = useListContacts({ companyId, page, pageSize: CONTACTS_PAGE_SIZE })
  const pii = useFieldPermission('contactPii')

  const columns: Column<ContactDto>[] = [
    {
      header: 'Name',
      cell: (contact) => (
        <Link
          to="/contacts/$contactId"
          params={{ contactId: contact.id }}
          className="text-primary font-semibold hover:underline"
        >
          {[contact.firstName, contact.lastName].filter(Boolean).join(' ')}
        </Link>
      ),
    },
    { header: 'Job title', cell: (contact) => contact.jobTitle ?? '—' },
    ...(pii.canRead
      ? [
          { header: 'Email', cell: (contact: ContactDto) => contact.email ?? '—' },
          { header: 'Phone', cell: (contact: ContactDto) => contact.mobile ?? contact.phone ?? '—' },
        ]
      : []),
  ]

  const newContactButton = (
    <PermissionGate scope="crm" action="create">
      <Button variant="outline" asChild>
        <Link to="/contacts/new" search={{ companyId }}>
          New contact
        </Link>
      </Button>
    </PermissionGate>
  )

  return (
    <section className="flex flex-col gap-3" aria-labelledby="company-contacts">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h2 id="company-contacts" className="text-xl font-semibold">
          Contacts
        </h2>
        {newContactButton}
      </div>
      {contacts.isPending ? <ListSkeleton rows={2} /> : null}
      {contacts.error ? (
        <ErrorState error={contacts.error} onRetry={() => void contacts.refetch()} />
      ) : null}
      {contacts.data && contacts.data.items.length === 0 ? (
        <Card>
          <EmptyState message="No contacts at this company yet." />
        </Card>
      ) : null}
      {contacts.data && contacts.data.items.length > 0 ? (
        <>
          <ResponsiveTable
            items={contacts.data.items}
            getKey={(contact) => contact.id}
            columns={columns}
          />
          <Pagination
            page={contacts.data.page}
            pageSize={contacts.data.pageSize}
            total={contacts.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}
    </section>
  )
}
