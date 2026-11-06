-- One-time database setup. Run as a Postgres superuser: psql -U postgres -f deploy/sql/init-roles.sql
-- cem_owner owns the schema and runs migrations; cem_app is what the API connects as.
-- cem_app is not the owner and has no BYPASSRLS, so row-level security applies to it.

CREATE ROLE cem_owner LOGIN PASSWORD 'change-me';
CREATE ROLE cem_app LOGIN PASSWORD 'change-me' NOSUPERUSER NOBYPASSRLS;
CREATE DATABASE cem OWNER cem_owner;

\connect cem

-- Repeat this block for every module schema (crm, sales, ...). Migrations later create the tables as cem_owner.
CREATE SCHEMA identity AUTHORIZATION cem_owner;
GRANT USAGE ON SCHEMA identity TO cem_app;
ALTER DEFAULT PRIVILEGES FOR ROLE cem_owner IN SCHEMA identity
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO cem_app;

-- Shared tables of the platform building block (audit log, outbox).
CREATE SCHEMA platform AUTHORIZATION cem_owner;
GRANT USAGE ON SCHEMA platform TO cem_app;
ALTER DEFAULT PRIVILEGES FOR ROLE cem_owner IN SCHEMA platform
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO cem_app;
