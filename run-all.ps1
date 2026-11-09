<#
.SYNOPSIS
    Runs the CEM API and web app together for local development. Ctrl+C stops both.

.EXAMPLE
    ./run-all.ps1                      # install what's missing, then start API + web
    ./run-all.ps1 -Migrate             # also apply database migrations first (as cem_owner)
    ./run-all.ps1 -Migrate -OwnerConnection "Host=localhost;Database=cem;Username=cem_owner;Password=..."

.NOTES
    One-time setup (see CLAUDE.md and docs/architecture/deployment.md):
      - Postgres: run deploy/sql/init-roles.sql as a superuser
      - API secrets (dotnet user-secrets, project src/Cem.Host):
          ConnectionStrings:Cem  -> connection string for cem_app
          Clerk:Authority        -> your Clerk Frontend API URL
      - Web: web/.env.local with VITE_CLERK_PUBLISHABLE_KEY
#>
param(
    # Apply EF migrations for every module before starting.
    [switch]$Migrate,

    # Owner connection used for migrations; defaults to the CEM_OWNER_CONNECTION environment variable.
    [string]$OwnerConnection = $env:CEM_OWNER_CONNECTION
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$hostProject = Join-Path $root 'src/Cem.Host'
$web = Join-Path $root 'web'

# Every DbContext that owns migrations: project path + context name.
$migrationTargets = @(
    @{ Project = 'src/Modules/Identity/Cem.Identity.Infrastructure'; Context = 'IdentityDbContext' },
    @{ Project = 'src/BuildingBlocks/Cem.Platform'; Context = 'PlatformDbContext' },
    @{ Project = 'src/Modules/Settings/Cem.Settings.Infrastructure'; Context = 'SettingsDbContext' },
    @{ Project = 'src/Modules/Crm/Cem.Crm.Infrastructure'; Context = 'CrmDbContext' }
)

function Write-Step([string]$message) { Write-Host "==> $message" -ForegroundColor Cyan }
function Write-Warn([string]$message) { Write-Host "    ! $message" -ForegroundColor Yellow }

function Assert-Command([string]$name, [string]$hint) {
    if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
        throw "'$name' was not found. $hint"
    }
}

function Test-Setup {
    Write-Step 'Checking setup'

    $secrets = dotnet user-secrets list --project $hostProject 2>$null | Out-String
    if ($secrets -notmatch 'ConnectionStrings:Cem') {
        Write-Warn 'ConnectionStrings:Cem is not set. The API will start, but database calls will fail.'
        Write-Warn "  dotnet user-secrets set ""ConnectionStrings:Cem"" ""Host=localhost;Database=cem;Username=cem_app;Password=..."" --project src/Cem.Host"
    }
    if ($secrets -notmatch 'Clerk:Authority') {
        Write-Warn 'Clerk:Authority is not set. Signed-in API calls will return 401.'
        Write-Warn '  dotnet user-secrets set "Clerk:Authority" "https://<your-app>.clerk.accounts.dev" --project src/Cem.Host'
    }

    $envFile = Join-Path $web '.env.local'
    if (-not (Test-Path $envFile)) {
        Copy-Item (Join-Path $web '.env.example') $envFile
        Write-Warn 'Created web/.env.local from .env.example. Add your VITE_CLERK_PUBLISHABLE_KEY to it.'
    }
}

function Install-Dependencies {
    Write-Step 'Restoring .NET tools'
    dotnet tool restore | Out-Null

    if (-not (Test-Path (Join-Path $web 'node_modules'))) {
        Write-Step 'Installing web packages (first run only)'
        npm ci --prefix $web
        if ($LASTEXITCODE -ne 0) { throw 'npm ci failed.' }
    }
}

function Invoke-Migrations {
    if (-not $OwnerConnection) {
        throw 'Migrations need the owner connection: pass -OwnerConnection or set CEM_OWNER_CONNECTION.'
    }

    foreach ($target in $migrationTargets) {
        Write-Step "Migrating $($target.Context)"
        dotnet ef database update `
            --project (Join-Path $root $target.Project) `
            --startup-project $hostProject `
            --context $target.Context `
            --connection $OwnerConnection
        if ($LASTEXITCODE -ne 0) { throw "Migration failed for $($target.Context)." }
    }
}

# Kills a process and its children (dotnet run and npm both spawn child processes).
function Stop-Tree($process) {
    if ($process -and -not $process.HasExited) {
        taskkill /PID $process.Id /T /F 2>$null | Out-Null
    }
}

Assert-Command 'dotnet' 'Install the .NET 10 SDK.'
Assert-Command 'npm' 'Install Node.js 24 or later.'

Test-Setup
Install-Dependencies
if ($Migrate) { Invoke-Migrations }

$api = $null
$webApp = $null
try {
    Write-Step 'Starting API   -> http://localhost:5080  (health: /health)'
    $api = Start-Process dotnet -ArgumentList 'run', '--project', $hostProject -NoNewWindow -PassThru

    Write-Step 'Starting web   -> http://localhost:5173'
    # Resolve npm's real path: it is npm.cmd with the Node installer but npm.exe with some version managers.
    $npm = (Get-Command npm -CommandType Application | Select-Object -First 1).Source
    $webApp = Start-Process $npm -ArgumentList 'run', 'dev', '--prefix', $web -NoNewWindow -PassThru

    Write-Host ''
    Write-Host 'Both are running. Press Ctrl+C to stop.' -ForegroundColor Green

    # Stay alive while both run; if either exits (e.g. a build error), stop the other too.
    while (-not $api.HasExited -and -not $webApp.HasExited) {
        Start-Sleep -Milliseconds 500
    }

    if ($api.HasExited) { Write-Warn "API stopped (exit code $($api.ExitCode))." }
    if ($webApp.HasExited) { Write-Warn "Web app stopped (exit code $($webApp.ExitCode))." }
}
finally {
    Write-Step 'Stopping'
    Stop-Tree $api
    Stop-Tree $webApp
}
