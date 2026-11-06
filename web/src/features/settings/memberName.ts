import type { MemberDto } from '@/lib/api/generated/models'

export function memberName(member: MemberDto): string {
  const name = [member.firstName, member.lastName].filter(Boolean).join(' ')
  return name || member.email || 'Unnamed member'
}
