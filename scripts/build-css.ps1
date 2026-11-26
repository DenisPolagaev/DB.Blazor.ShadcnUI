#Requires -Version 7
<#
.SYNOPSIS
    Rebuilds the shipped stylesheet (src/DP.ShadcnBlazor/wwwroot/dp-shadcn.css).

.DESCRIPTION
    Tailwind v4 scans the library's .razor/.cs files (the `@source` directives in
    styles/dp-shadcn.css) and emits only the utilities the components actually use. The output is
    committed so `dotnet build`, `dotnet pack` and every NuGet consumer work without Node.

    `npm install` is only needed once (or after bumping the pinned Tailwind version).
#>
[CmdletBinding()]
param(
    [switch]$Install
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

if ($Install -or -not (Test-Path (Join-Path $root 'node_modules/.bin/tailwindcss.cmd'))) {
    npm install --no-audit --no-fund
}
if ($LASTEXITCODE -ne 0) { throw 'npm install failed' }

npm run build
if ($LASTEXITCODE -ne 0) { throw 'tailwind build failed' }

Write-Host "Wrote $root/src/DP.ShadcnBlazor/wwwroot/dp-shadcn.css"
