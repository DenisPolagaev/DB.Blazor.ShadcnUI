# Compares the cn-* component tokens of a shadcn/ui style preset with the ported sources.
# Reports cn-* tokens that exist in the preset's registry sources but nowhere in the port
# (a real omission), and tokens a ported component uses that its own source does not.
#
# The preset's markup carries semantic `cn-*` classes; the actual styling lives in the preset
# stylesheet (styles/style-nova.css). Comparing cn-* tokens therefore checks that every component
# part of the port is wired to its preset class, which is what a style migration is about - the
# utility classes move into the stylesheet and are not repeated in the C# port.
#
# Usage: ./scripts/check-parity.ps1 -ReferenceDir <dir with the preset's *.tsx files>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ReferenceDir
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$srcRoot = Join-Path $root 'src/DB.Blazor.ShadcnUI'

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
    'command'       = 'Command'
    'context-menu'  = 'ContextMenu'
    'dialog'        = 'Dialog'
    'drawer'        = 'Drawer'
    'dropdown-menu' = 'DropdownMenu'
    'field'         = 'Field'
    'hover-card'    = 'HoverCard'
    'input'         = 'Input'
    'input-group'   = 'InputGroup'
    'input-otp'     = 'InputOTP'
    'kbd'           = 'Kbd'
    'label'         = 'Label'
    'menubar'       = 'Menubar'
    'native-select' = 'NativeSelect'
    'pagination'    = 'Pagination'
    'popover'       = 'Popover'
    'progress'      = 'Progress'
    'radio-group'   = 'RadioGroup'
    'select'        = 'Select'
    'separator'     = 'Separator'
    'sheet'         = 'Sheet'
    'skeleton'      = 'Skeleton'
    'slider'        = 'Slider'
    'spinner'       = 'Spinner'
    'switch'        = 'Switch'
    'table'         = 'Table'
    'tabs'          = 'Tabs'
    'textarea'      = 'Textarea'
    'tooltip'       = 'Tooltip'
}

# Tokens the preset stylesheet provides for a component the registry source does not name
# (the style's separator.tsx writes utilities, while style-nova.css also carries cn-separator).
$knownExtras = @(
    'cn-separator',
    'cn-separator-horizontal',
    'cn-separator-vertical'
)

function Get-CnTokens([string]$path) {
    $content = Get-Content $path -Raw
    return @([regex]::Matches($content, 'cn-[a-z0-9-]+') | ForEach-Object { $_.Value } | Sort-Object -Unique)
}

$portFiles = @(Get-ChildItem $srcRoot -Recurse -Include *.razor, *.cs -File | Select-Object -ExpandProperty FullName)
$portTokens = New-Object 'System.Collections.Generic.HashSet[string]'
foreach ($file in $portFiles) {
    foreach ($token in (Get-CnTokens $file)) { [void]$portTokens.Add($token) }
}

$totalMissing = 0
foreach ($name in $map.Keys) {
    $tsx = Join-Path $ReferenceDir "$name.tsx"
    if (-not (Test-Path $tsx)) { Write-Host "no reference for $name" -ForegroundColor DarkGray; continue }

    $referenceTokens = Get-CnTokens $tsx
    $missing = @($referenceTokens | Where-Object { -not $portTokens.Contains($_) } | Sort-Object)

    if ($missing.Count -gt 0) {
        $totalMissing += $missing.Count
        Write-Host "`n[$name] $($missing.Count) cn token(s) from the preset are missing:" -ForegroundColor Yellow
        $missing | ForEach-Object { Write-Host "  $_" }
    }

    # Reverse direction: a cn token a ported component uses that its own preset source never names.
    $folder = Join-Path $srcRoot "Components/$($map[$name])"
    $variantFile = Join-Path $srcRoot "Variants/$($map[$name])Variants.cs"
    $ownFiles = @()
    if (Test-Path $folder) { $ownFiles += @(Get-ChildItem $folder -Recurse -Include *.razor, *.cs -File | Select-Object -ExpandProperty FullName) }
    if (Test-Path $variantFile) { $ownFiles += $variantFile }
    if ($ownFiles.Count -eq 0) { continue }

    $ownTokens = @($ownFiles | ForEach-Object { Get-CnTokens $_ } | Sort-Object -Unique)
    $extra = @($ownTokens | Where-Object { $referenceTokens -notcontains $_ } | Where-Object { $knownExtras -notcontains $_ } | Sort-Object)

    if ($extra.Count -gt 0) {
        Write-Host "[$name] $($extra.Count) cn token(s) the preset source does not name:" -ForegroundColor DarkYellow
        $extra | ForEach-Object { Write-Host "  $_" }
    }
}

if ($totalMissing -eq 0) {
    Write-Host "`nAll preset cn tokens are present in the port ($($map.Count) components)." -ForegroundColor Green
}
else {
    Write-Host "`n$totalMissing missing cn token(s) across $($map.Count) components." -ForegroundColor Yellow
    exit 1
}
