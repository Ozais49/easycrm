import { Link } from '@tanstack/react-router'
import { useCallback, useState } from 'react'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { Pagination } from '@/components/Pagination'
import { PermissionGate } from '@/components/PermissionGate'
import { ResponsiveTable, type Column } from '@/components/ResponsiveTable'
import { Button } from '@/components/ui/button'
import { SearchInput } from '@/features/crm/components/SearchInput'
import { TagFilter } from '@/features/crm/components/TagFilter'
import { contactName } from '@/features/crm/contactName'
import { useCustomFieldDefinitions } from '@/features/crm/hooks/useCustomFieldDefinitions'
import { customFieldColumns, tagsColumn } from '@/features/crm/listColumns'
import { useFieldPermission } from '@/features/identity/hooks/useFieldPermission'
import { useListContacts } from '@/lib/api/generated/endpoints'
import type { ContactDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

export function ContactsPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [tagId, setTagId] = useState('')
  const definitions = useCustomFieldDefinitions('contact')
  const pii = useFieldPermission('contactPii')
  const contacts = useListContacts({
    page,
    pageSize: PAGE_SIZE,
    q: search === '' ? undefined : search,
    tagId: tagId === '' ? undefined : tagId,
  })

  // A new search starts on the first page.
  const onSearch = useCallback((text: string) => {
    setSearch(text)
    setPage(1)
  }, [])

  const columns: Column<ContactDto>[] = [
    {
      header: 'Name',
      cell: (contact) => (
        <Link
          to="/contacts/$contactId"
          params={{ contactId: contact.id }}
          className="text-primary font-semibold hover:underline"
        >
          {contactName(contact)}
        </Link>
      ),
    },
    {
      header: 'Company',
      cell: (contact) =>
        contact.companyId && contact.companyName ? (
          <Link
            to="/companies/$companyId"
            params={{ companyId: contact.companyId }}
            className="text-primary hover:underline"
          >
            {contact.companyName}
          </Link>
        ) : (
          '—'
        ),
    },
    { header: 'Job title', cell: (contact) => contact.jobTitle ?? '—' },
    // Contact details are absent from the response for users without access: leave the columns out.
    ...(pii.canRead
      ? [
          { header: 'Email', cell: (contact: ContactDto) => contact.email ?? '—' },
          {
            header: 'Phone',
            cell: (contact: ContactDto) => contact.mobile ?? contact.phone ?? '—',
          },
        ]
      : []),
    tagsColumn<ContactDto>(),
    ...customFieldColumns<ContactDto>(definitions.data),
  ]

  const newContactButton = (
    <PermissionGate scope="crm" action="create">
      <Button asChild>
        <Link to="/contacts/new">New contact</Link>
      </Button>
    </PermissionGate>
  )

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Contacts"
        description="The people you work with."
        action={newContactButton}
      />
      <div className="flex flex-col gap-3 sm:flex-row">
        <SearchInput label="Search contacts" placeholder="Search by name" onSearch={onSearch} />
        <TagFilter
          value={tagId}
          onChange={(id) => {
            setTagId(id)
            setPage(1)
          }}
        />
      </div>
      {contacts.isPending ? <ListSkeleton /> : null}
      {contacts.error ? (
        <ErrorState error={contacts.error} onRetry={() => void contacts.refetch()} />
      ) : null}
      {contacts.data && contacts.data.items.length === 0 ? (
        <EmptyState
          message={
            search === '' && tagId === ''
              ? 'No contacts yet. Add the first one.'
              : 'No contacts match. Try a different name or tag.'
          }
          action={search === '' && tagId === '' ? newContactButton : undefined}
        />
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
    </div>
  )
}
