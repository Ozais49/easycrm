import { PageHeader } from '@/components/PageHeader'
import { CompanyForm } from '@/features/crm/components/CompanyForm'

export function CompanyCreatePage() {
  return (
    <div className="flex flex-col gap-6">
      <PageHeader title="New company" />
      <CompanyForm />
    </div>
  )
}
