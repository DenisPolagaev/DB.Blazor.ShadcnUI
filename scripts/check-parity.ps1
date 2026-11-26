# Compares the class tokens of the shadcn/ui v4 registry with the ported sources.
# Reports class tokens that exist in the registry but nowhere in the port (a real omission).
# Usage: ./scripts/check-parity.ps1 -ReferenceDir <dir with new-york-v4 *.tsx>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ReferenceDir
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$srcRoot = Join-Path $root 'src/DP.ShadcnBlazor'

$map = [ordered]@{
    'accordion'     = 'Accordion'
    'aspect-ratio'  = 'AspectRatio'
    'collapsible'   = 'Collapsible'
    'empty'         = 'Empty'
    'scroll-area'   = 'ScrollArea'
    'toggle'        = 'Toggle'
    'toggle-group'  = 'ToggleGroup'
    'alert'         = 'Alert'
    'alert-dialog'  = 'AlertDialog'
    'avatar'        = 'Avatar'
    'badge'         = 'Badge'
    'button-group'  = 'ButtonGroup'
    'item'          = 'Item'
    'breadcrumb'    = 'Breadcrumb'
    'button'        = 'Button'
    'card'          = 'Card'
    'checkbox'      = 'Checkbox'
    'dialog'        = 'Dialog'
    'dropdown-menu' = 'DropdownMenu'
    'input'         = 'Input'
    'kbd'           = 'Kbd'
    'label'         = 'Label'
    'native-select' = 'NativeSelect'
    'pagination'    = 'Pagination'
    'popover'       = 'Popover'
    'progress'      = 'Progress'
    'radio-group'   = 'RadioGroup'
    'separator'     = 'Separator'
    'sheet'         = 'Sheet'
    'skeleton'      = 'Skeleton'
    'spinner'       = 'Spinner'
    'switch'        = 'Switch'
    'table'         = 'Table'
    'tabs'          = 'Tabs'
    'textarea'      = 'Textarea'
    'tooltip'       = 'Tooltip'
}

function Get-QuotedStrings([string]$path) {
    $content = Get-Content $path -Raw
    $found = [regex]::Matches($content, '"([^"\\\r\n]{3,})"')
    return @($found | ForEach-Object { $_.Groups[1].Value })
}

function Get-ClassTokens([string[]]$texts) {
    $tokens = New-Object 'System.Collections.Generic.HashSet[string]'
    foreach ($text in $texts) {
        if (-not $text) { continue }
        foreach ($raw in ($text -split '\s+')) {
            $token = $raw.Trim()
            if ($token.Length -eq 0) { continue }
            if ($token -match '[\$\{\}`]') { continue }
            # Only tokens that look like Tailwind utilities.
            if ($token -notmatch '^[a-z\[@\*!-]' -or $token -notmatch '-') { continue }
            [void]$tokens.Add($token)
        }
    }
    return $tokens
}

$portFiles = @(Get-ChildItem $srcRoot -Recurse -Include *.razor, *.cs -File | Select-Object -ExpandProperty FullName)
$portTexts = @($portFiles | ForEach-Object { Get-QuotedStrings $_ })
$portTokens = Get-ClassTokens $portTexts

$totalMissing = 0
$allReference = @($map.Keys | ForEach-Object { $tsx = Join-Path $ReferenceDir "$_.tsx"; if (Test-Path $tsx) { Get-ClassTokens (Get-QuotedStrings $tsx) } })
foreach ($name in $map.Keys) {
    $tsx = Join-Path $ReferenceDir "$name.tsx"
    if (-not (Test-Path $tsx)) { Write-Host "no reference for $name" -ForegroundColor DarkGray; continue }

    $referenceTokens = Get-ClassTokens (Get-QuotedStrings $tsx)
    $missing = @($referenceTokens | Where-Object { -not $portTokens.Contains($_) } |
        Where-Object { $_ -notmatch '^(client|use|radix-ui|lucide-react|react|class-variance-authority)$' } |
        Sort-Object)

    if ($missing.Count -gt 0) {
        $totalMissing += $missing.Count
        Write-Host "`n[$name] $($missing.Count) token(s) from the registry are missing:" -ForegroundColor Yellow
        $missing | ForEach-Object { Write-Host "  $_" }
    }

    # Reverse direction: a utility the component uses that never appears in any reference file.
    $folder = Join-Path $srcRoot "Components/$($map[$name])"
    $variantFile = Join-Path $srcRoot "Variants/$($map[$name])Variants.cs"
    $ownFiles = @()
    if (Test-Path $folder) { $ownFiles += @(Get-ChildItem $folder -Recurse -Include *.razor, *.cs -File | Select-Object -ExpandProperty FullName) }
    if (Test-Path $variantFile) { $ownFiles += $variantFile }
    if ($ownFiles.Count -eq 0) { continue }

    $ownTexts = @($ownFiles | ForEach-Object { Get-QuotedStrings $_ })
    $ownTokens = @(Get-ClassTokens $ownTexts)
    $extra = @($ownTokens | Where-Object { $allReference -notcontains $_ } |
        Where-Object { $_ -match '^[a-z]' } |
        Where-Object { $_ -notmatch '^(data-slot|aria-|role)' } |
        Sort-Object)

    if ($extra.Count -gt 0) {
        Write-Host "[$name] $($extra.Count) token(s) not found in any reference file:" -ForegroundColor DarkYellow
        $extra | ForEach-Object { Write-Host "  $_" }
    }
}

Write-Host "`n$totalMissing missing token(s) across $($map.Count) components."
