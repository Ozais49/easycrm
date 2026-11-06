import { toast } from 'sonner'

/** An RFC 7807 problem response, parsed. Status 0 means the request never reached the server. */
export class ApiError extends Error {
  readonly status: number
  readonly code: string | undefined
  readonly title: string | undefined
  readonly fieldErrors: Record<string, string[]>

  constructor(
    status: number,
    code: string | undefined,
    title: string | undefined,
    fieldErrors: Record<string, string[]>,
  ) {
    super(title ?? `Request failed (${status})`)
    this.name = 'ApiError'
    this.status = status
    this.code = code
    this.title = title
    this.fieldErrors = fieldErrors
  }
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null
}

function parseFieldErrors(value: unknown): Record<string, string[]> {
  if (!isRecord(value)) return {}
  const result: Record<string, string[]> = {}
  for (const [key, messages] of Object.entries(value)) {
    if (Array.isArray(messages)) {
      result[key] = messages.filter((m): m is string => typeof m === 'string')
    }
  }
  return result
}

export function parseProblem(status: number, body: unknown): ApiError {
  const problem = isRecord(body) ? body : {}
  return new ApiError(
    status,
    typeof problem.code === 'string' ? problem.code : undefined,
    typeof problem.title === 'string' ? problem.title : undefined,
    parseFieldErrors(problem.errors),
  )
}

const MESSAGES_BY_CODE: Record<string, string> = {
  'roles.last_admin':
    'Your organization needs at least one active Admin. Give the Admin role to someone else first.',
  'roles.in_use': 'This role is still assigned to members. Remove it from them first.',
  'roles.name_taken': 'Another role already has this name. Pick a different name.',
  'roles.system_role': "Built-in roles can't be renamed or deleted.",
  'teams.name_taken': 'Another team already has this name. Pick a different name.',
  'permission.denied': "You don't have permission to do this. Ask an admin for access.",
  'field.forbidden': "You don't have permission to change one of these fields.",
  'tenant.required': 'Select an organization to continue.',
  'validation.failed': 'Some details are not valid. Check the form and try again.',
}

/** What happened + what to do. Never exposes status codes or error codes. */
export function friendlyMessage(error: unknown): string {
  if (error instanceof ApiError) {
    const byCode = error.code ? MESSAGES_BY_CODE[error.code] : undefined
    if (byCode) return byCode
    if (error.status === 0) return "Couldn't reach the server. Check your connection and try again."
    if (error.status === 401) return 'Your session has expired. Sign in again to continue.'
    if (error.status === 403)
      return "You don't have permission to do this. Ask an admin for access."
    if (error.status === 404)
      return "We couldn't find that. It may have been removed. Refresh and try again."
    if (error.status === 409) return 'This changed since you opened it. Refresh and try again.'
    if (error.status < 500) return 'Some details are not valid. Check them and try again.'
  }
  return 'Something went wrong on our side. Try again in a moment.'
}

export function toastApiError(error: unknown) {
  toast.error(friendlyMessage(error))
}
