param(
    [string]$ServerName,
    [string]$DatabaseName,
    [string]$AdminUser,
    [string]$AdminPassword
)

$ErrorActionPreference = "Stop"

$SchemaPath = Join-Path $PSScriptRoot "..\schema\Pharmacy_Schema.sql"
$SeedPath = Join-Path $PSScriptRoot "..\schema\Pharmacy_SeedData.sql"

Write-Host "================================="
Write-Host "Initializing database..."
Write-Host "Server: $ServerName"
Write-Host "Database: $DatabaseName"
Write-Host "================================="

if (-not (Test-Path $SchemaPath))
{
    throw "Schema file not found: $SchemaPath"
}

if (-not (Test-Path $SeedPath))
{
    throw "Seed file not found: $SeedPath"
}

Write-Host ""
Write-Host "Running schema..."

sqlcmd `
    -S "$ServerName.database.windows.net" `
    -d "$DatabaseName" `
    -U "$AdminUser" `
    -P "$AdminPassword" `
    -i "$SchemaPath"

if ($LASTEXITCODE -ne 0)
{
    throw "Schema deployment failed."
}

Write-Host ""
Write-Host "Schema deployed successfully."

Write-Host ""
Write-Host "Running seed data..."

sqlcmd `
    -S "$ServerName.database.windows.net" `
    -d "$DatabaseName" `
    -U "$AdminUser" `
    -P "$AdminPassword" `
    -i "$SeedPath"

if ($LASTEXITCODE -ne 0)
{
    throw "Seed deployment failed."
}

Write-Host ""
Write-Host "Database initialized successfully."