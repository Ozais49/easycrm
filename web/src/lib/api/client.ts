import { apiBaseUrl } from '../env'
import { ApiError, parseProblem } from './errors'

type TokenGetter = () => Promise<string | null>

let getToken: TokenGetter = () => Promise.resolve(null)

/** Registered once from the Clerk hook (see app/ApiAuthBridge.tsx). */
export function registerTokenGetter(getter: TokenGetter) {
  getToken = getter
}

/** The request shape orval's generated hooks pass to the mutator. */
export type RequestConfig = {
  url: string
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'
  params?: Record<string, unknown>
  data?: unknown
  headers?: Record<string, string>
  signal?: AbortSignal
}

/** Lets the generated hooks type their error as ApiError, whatever the declared problem body. */
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export type ErrorType<_Body> = ApiError

function buildUrl(url: string, params: RequestConfig['params']): string {
  const search = new URLSearchParams()
  for (const [key, value] of Object.entries(params ?? {})) {
    if (value !== undefined && value !== null) search.set(key, String(value))
  }
  const query = search.toString()
  return `${apiBaseUrl}${url}${query ? `?${query}` : ''}`
}

async function readBody(response: Response): Promise<unknown> {
  const text = await response.text()
  if (!text) return undefined
  try {
    return JSON.parse(text)
  } catch {
    return undefined
  }
}

/** The orval mutator: attaches the Clerk token and turns problem responses into ApiError. */
export async function apiFetch<T>(config: RequestConfig): Promise<T> {
  const token = await getToken()
  const headers: Record<string, string> = { ...config.headers }
  if (token) headers.Authorization = `Bearer ${token}`
  if (config.data !== undefined) headers['Content-Type'] = 'application/json'

  let response: Response
  try {
    response = await fetch(buildUrl(config.url, config.params), {
      method: config.method,
      headers,
      body: config.data === undefined ? undefined : JSON.stringify(config.data),
      signal: config.signal,
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error
    throw new ApiError(0, undefined, undefined, {})
  }

  const body = await readBody(response)
  if (!response.ok) throw parseProblem(response.status, body)
  return body as T
}
