import { useSearch } from '@tanstack/react-router'
import { PageHeader } from '@/components/PageHeader'
import { ContactForm } from '@/features/crm/components/ContactForm'

export function ContactCreatePage() {
  const { companyId } = useSearch({ from: '/contacts/new' })

  return (
    <div className="flex flex-col gap-6">
      <PageHeader title="New contact" />
      <ContactForm companyId={companyId} />
    </div>
  )
}
