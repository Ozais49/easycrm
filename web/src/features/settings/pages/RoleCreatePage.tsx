import { PageHeader } from '@/components/PageHeader'
import { RoleForm } from '@/features/settings/components/RoleForm'

export function RoleCreatePage() {
  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="New role"
        description="Choose what people with this role can see and do."
      />
      <RoleForm />
    </div>
  )
}
