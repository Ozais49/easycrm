import {
  createRootRoute,
  createRoute,
  createRouter,
  lazyRouteComponent,
  redirect,
} from '@tanstack/react-router'
import { AppShell } from '@/app/AppShell'
import { RouteError } from '@/app/RouteError'
import { RouteNotFound } from '@/app/RouteNotFound'
import { HomePage } from '@/features/home/pages/HomePage'
import { AdminGuard } from '@/features/settings/components/AdminGuard'

const rootRoute = createRootRoute({
  component: AppShell,
  errorComponent: RouteError,
  notFoundComponent: RouteNotFound,
})

const homeRoute = createRoute({ getParentRoute: () => rootRoute, path: '/', component: HomePage })

// Companies and contacts are lazy-loaded; reading needs Crm:Read, which the navigation already hides without.
const companiesRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'companies',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/CompaniesPage'),
    'CompaniesPage',
  ),
})

const newCompanyRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'companies/new',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/CompanyCreatePage'),
    'CompanyCreatePage',
  ),
})

const companyRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'companies/$companyId',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/CompanyDetailPage'),
    'CompanyDetailPage',
  ),
})

const editCompanyRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'companies/$companyId/edit',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/CompanyEditPage'),
    'CompanyEditPage',
  ),
})

const contactsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'contacts',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/ContactsPage'),
    'ContactsPage',
  ),
})

// ?companyId= pre-selects the company, used by "New contact" on a company page.
const newContactRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'contacts/new',
  validateSearch: (search: Record<string, unknown>): { companyId?: string } =>
    typeof search.companyId === 'string' ? { companyId: search.companyId } : {},
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/ContactCreatePage'),
    'ContactCreatePage',
  ),
})

const contactRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'contacts/$contactId',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/ContactDetailPage'),
    'ContactDetailPage',
  ),
})

const editContactRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'contacts/$contactId/edit',
  component: lazyRouteComponent(
    () => import('@/features/crm/pages/ContactEditPage'),
    'ContactEditPage',
  ),
})

// The settings screens are lazy-loaded: they are only needed once someone opens Settings.
const settingsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'settings',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/SettingsLayout'),
    'SettingsLayout',
  ),
})

const settingsIndexRoute = createRoute({
  getParentRoute: () => settingsRoute,
  path: '/',
  beforeLoad: () => {
    throw redirect({ to: '/settings/general' })
  },
})

const generalRoute = createRoute({
  getParentRoute: () => settingsRoute,
  path: 'general',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/GeneralPage'),
    'GeneralPage',
  ),
})

// Members, roles, teams, tags, custom fields and the audit log need Settings:Read.
const adminRoute = createRoute({
  getParentRoute: () => settingsRoute,
  id: 'admin',
  component: AdminGuard,
})

const membersRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'members',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/MembersPage'),
    'MembersPage',
  ),
})

const rolesRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'roles',
  component: lazyRouteComponent(() => import('@/features/settings/pages/RolesPage'), 'RolesPage'),
})

const newRoleRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'roles/new',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/RoleCreatePage'),
    'RoleCreatePage',
  ),
})

const editRoleRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'roles/$roleId',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/RoleEditPage'),
    'RoleEditPage',
  ),
})

const teamsRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'teams',
  component: lazyRouteComponent(() => import('@/features/settings/pages/TeamsPage'), 'TeamsPage'),
})

const tagsRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'tags',
  component: lazyRouteComponent(() => import('@/features/settings/pages/TagsPage'), 'TagsPage'),
})

const customFieldsRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'custom-fields',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/CustomFieldsPage'),
    'CustomFieldsPage',
  ),
})

const auditLogRoute = createRoute({
  getParentRoute: () => adminRoute,
  path: 'audit-log',
  component: lazyRouteComponent(
    () => import('@/features/settings/pages/AuditLogPage'),
    'AuditLogPage',
  ),
})

const routeTree = rootRoute.addChildren([
  homeRoute,
  companiesRoute,
  newCompanyRoute,
  companyRoute,
  editCompanyRoute,
  contactsRoute,
  newContactRoute,
  contactRoute,
  editContactRoute,
  settingsRoute.addChildren([
    settingsIndexRoute,
    generalRoute,
    adminRoute.addChildren([
      membersRoute,
      rolesRoute,
      newRoleRoute,
      editRoleRoute,
      teamsRoute,
      tagsRoute,
      customFieldsRoute,
      auditLogRoute,
    ]),
  ]),
])

export const router = createRouter({ routeTree, defaultPreload: 'intent' })

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}
