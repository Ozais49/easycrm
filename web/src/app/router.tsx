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

// Members, roles, teams and the audit log need Settings:Read.
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
  settingsRoute.addChildren([
    settingsIndexRoute,
    generalRoute,
    adminRoute.addChildren([
      membersRoute,
      rolesRoute,
      newRoleRoute,
      editRoleRoute,
      teamsRoute,
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
