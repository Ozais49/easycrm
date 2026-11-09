import { useParams } from '@tanstack/react-router'
import { ErrorState } from '@/components/ErrorState'
import { ListSkeleton } from '@/components/ListSkeleton'
import { PageHeader } from '@/components/PageHeader'
import { ContactForm } from '@/features/crm/components/ContactForm'
import { contactName } from '@/features/crm/contactName'
import { useGetContact } from '@/lib/api/generated/endpoints'

export function ContactEditPage() {
  const { contactId } = useParams({ from: '/contacts/$contactId/edit' })
  const { data: contact, error, isPending, refetch } = useGetContact(contactId)

  return (
    <div className="flex flex-col gap-6">
      <PageHeader title={contact ? `Edit ${contactName(contact)}` : 'Edit contact'} />
      {isPending ? <ListSkeleton rows={5} /> : null}
      {error ? <ErrorState error={error} onRetry={() => void refetch()} /> : null}
      {contact ? <ContactForm key={contact.id} contact={contact} /> : null}
    </div>
  )
}
