// Domain vocabulary of the permission matrix (ADR-0016). Request and response shapes come from the generated API client.
export const SCOPES = [
  'crm',
  'calendar',
  'sales',
  'billing',
  'projects',
  'portfolio',
  'inbox',
  'settings',
] as const
export const ACTIONS = ['create', 'read', 'edit', 'delete'] as const
export const FIELD_GROUPS = ['pricing', 'financial', 'contactPii'] as const

export type Scope = (typeof SCOPES)[number]
export type Action = (typeof ACTIONS)[number]
export type FieldGroup = (typeof FIELD_GROUPS)[number]

/** The one place with human labels for scopes, actions, field groups and levels. */
export const SCOPE_LABELS: Record<Scope, string> = {
  crm: 'Companies & contacts',
  calendar: 'Calendar',
  sales: 'Leads & deals',
  billing: 'Quotes & invoices',
  projects: 'Projects & deliverables',
  portfolio: 'Portfolio',
  inbox: 'Inbox',
  settings: 'Settings',
}

export const ACTION_LABELS: Record<Action, string> = {
  create: 'Create',
  read: 'View',
  edit: 'Edit',
  delete: 'Delete',
}

export const FIELD_GROUP_LABELS: Record<FieldGroup, string> = {
  pricing: 'Pricing',
  financial: 'Financial',
  contactPii: 'Contact details',
}

export const LEVEL_LABELS = {
  none: 'None',
  own: 'Own',
  team: 'Team',
  all: 'All',
} as const
