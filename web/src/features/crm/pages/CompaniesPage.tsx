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
import { useListCompanies } from '@/lib/api/generated/endpoints'
import type { CompanyDto } from '@/lib/api/generated/models'

const PAGE_SIZE = 25

const columns: Column<CompanyDto>[] = [
  {
    header: 'Name',
    cell: (company) => (
      <Link
        to="/companies/$companyId"
        params={{ companyId: company.id }}
        className="text-primary font-semibold hover:underline"
      >
        {company.name}
      </Link>
    ),
  },
  { header: 'Industry', cell: (company) => company.industry ?? '—' },
  { header: 'City', cell: (company) => company.address.city ?? '—' },
  { header: 'Phone', cell: (company) => company.phone ?? '—' },
]

export function CompaniesPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const companies = useListCompanies({
    page,
    pageSize: PAGE_SIZE,
    q: search === '' ? undefined : search,
  })

  // A new search starts on the first page; a stable callback keeps the search box from re-firing.
  const onSearch = useCallback((text: string) => {
    setSearch(text)
    setPage(1)
  }, [])

  const newCompanyButton = (
    <PermissionGate scope="crm" action="create">
      <Button asChild>
        <Link to="/companies/new">New company</Link>
      </Button>
    </PermissionGate>
  )

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Companies"
        description="The organizations you work with."
        action={newCompanyButton}
      />
      <SearchInput label="Search companies" placeholder="Search by name" onSearch={onSearch} />
      {companies.isPending ? <ListSkeleton /> : null}
      {companies.error ? (
        <ErrorState error={companies.error} onRetry={() => void companies.refetch()} />
      ) : null}
      {companies.data && companies.data.items.length === 0 ? (
        <EmptyState
          message={
            search === ''
              ? 'No companies yet. Add the first one.'
              : 'No companies match your search. Try a different name.'
          }
          action={search === '' ? newCompanyButton : undefined}
        />
      ) : null}
      {companies.data && companies.data.items.length > 0 ? (
        <>
          <ResponsiveTable
            items={companies.data.items}
            getKey={(company) => company.id}
            columns={columns}
          />
          <Pagination
            page={companies.data.page}
            pageSize={companies.data.pageSize}
            total={companies.data.total}
            onPageChange={setPage}
          />
        </>
      ) : null}
    </div>
  )
}
