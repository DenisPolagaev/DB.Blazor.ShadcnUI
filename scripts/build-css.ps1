#Requires -Version 7
<#
.SYNOPSIS
    Rebuilds both committed stylesheets: the library bundle and the docs demo bundle.

.DESCRIPTION
    Tailwind v4 scans the library's .razor/.cs files and the demo's pages (the `@source`
    directives in `styles/dp-shadcn.css` and `samples/DP.ShadcnBlazor.Demo/styles/docs.css`).
    Both outputs are committed so `dotnet build`, `dotnet pack` and every NuGet consumer work
    without Node.

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
if ($LASTEXITCODE -ne 0) { throw 'library tailwind build failed' }

npm run build:demo
if ($LASTEXITCODE -ne 0) { throw 'demo tailwind build failed' }

Write-Host "Wrote $root/src/DP.ShadcnBlazor/wwwroot/dp-shadcn.css"
Write-Host "Wrote $root/samples/DP.ShadcnBlazor.Demo/wwwroot/docs.css"
