import { useParams } from '@tanstack/react-router'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { CompanyForm } from '@/features/crm/components/CompanyForm'
import { useGetCompany } from '@/lib/api/generated/endpoints'

export function CompanyEditPage() {
  const { companyId } = useParams({ from: '/companies/$companyId/edit' })
  const { data: company, error, isPending, refetch } = useGetCompany(companyId)

  return (
    <div className="flex flex-col gap-6">
      <PageHeader title={company ? `Edit ${company.name}` : 'Edit company'} />
      {isPending ? <ListSkeleton rows={5} /> : null}
      {error ? <ErrorState error={error} onRetry={() => void refetch()} /> : null}
      {company ? <CompanyForm key={company.id} company={company} /> : null}
    </div>
  )
}
