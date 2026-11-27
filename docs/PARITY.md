# Parity with shadcn/ui v4

This page is the checklist used while porting and the place to look when a visual difference shows
up. Every class string below was copied from the `new-york-v4` registry of
<https://github.com/shadcn-ui/ui> (`registry/new-york-v4/ui/*.tsx`).

## Markup

* Every component renders the same `data-slot` names as the React original: `button`, `badge`,
  `card`, `card-header`, `card-title`, `card-description`, `card-action`, `card-content`,
  `card-footer`, `alert`, `alert-title`, `alert-description`, `avatar`, `avatar-image`,
  `avatar-fallback`, `avatar-badge`, `avatar-group`, `avatar-group-count`, `separator`, `skeleton`,
  `progress`, `progress-indicator`, `kbd`, `kbd-group`, `input`, `textarea`, `label`, `checkbox`,
  `checkbox-indicator`, `switch`, `switch-thumb`, `radio-group`, `radio-group-item`,
  `radio-group-indicator`, `native-select`, `native-select-wrapper`, `native-select-icon`,
  `native-select-option`, `native-select-optgroup`, `table-container`, `table`, `table-header`,
  `table-body`, `table-footer`, `table-row`, `table-head`, `table-cell`, `table-caption`, `tabs`,
  `tabs-list`, `tabs-trigger`, `tabs-content`, `accordion`, `accordion-item`, `accordion-trigger`,
  `accordion-content`, `breadcrumb`, `breadcrumb-list`, `breadcrumb-item`, `breadcrumb-link`,
  `breadcrumb-page`, `breadcrumb-separator`, `breadcrumb-ellipsis`, `pagination`,
  `pagination-content`, `pagination-item`, `pagination-link`, `pagination-ellipsis`, `dialog-portal`,
  `dialog-overlay`, `dialog-content`, `dialog-close`, `dialog-header`, `dialog-footer`,
  `dialog-title`, `dialog-description`, `alert-dialog-*` (the same set), `sheet-portal`,
  `sheet-overlay`, `sheet-content`, `sheet-close`, `sheet-header`, `sheet-footer`, `sheet-title`,
  `sheet-description`, `dropdown-menu-content`, `dropdown-menu-item`, `dropdown-menu-checkbox-item`,
  `dropdown-menu-radio-group`, `dropdown-menu-radio-item`, `dropdown-menu-label`,
  `dropdown-menu-separator`, `dropdown-menu-shortcut`, `dropdown-menu-group`,
  `dropdown-menu-sub-trigger`, `dropdown-menu-sub-content`, `popover-content`, `popover-header`,
  `popover-title`, `popover-description`, `popover-anchor`, `tooltip-content`, `tooltip-arrow`.
* `data-state` is `open`/`closed` on overlays, `checked`/`unchecked` on checkboxes, switches and
  radio items, `active`/`inactive` on tab triggers.
* There is no `Selected` prop on `TableRow` (the React table is plain HTML): pass
  `data-state="selected"` as an attribute, exactly like the shadcn examples do.
* The port exposes no parameters the original does not have. Two-way binding is the one addition —
  `Value`/`ValueChanged`, `Checked`/`CheckedChanged`, `Open`/`OpenChanged` map React's
  `value`/`onValueChange`, `checked`/`onCheckedChange`, `open`/`onOpenChange` onto `@bind-`.
* `data-variant` / `data-size` / `data-orientation` / `data-inset` / `data-side` carry the same
  values as the React original.

## Variants (C# → shadcn value)

| Component | C# | shadcn |
| --- | --- | --- |
| `Button` | `ButtonVariant.Default` | `default` |
| | `.Destructive` / `.Outline` / `.Secondary` / `.Ghost` / `.Link` | same names |
| `Button` size | `ButtonSize.Default`, `.Xs`, `.Sm`, `.Lg`, `.Icon`, `.IconXs`, `.IconSm`, `.IconLg` | `default`, `xs`, `sm`, `lg`, `icon`, `icon-xs`, `icon-sm`, `icon-lg` |
| `Badge` | `BadgeVariant.Default`, `.Secondary`, `.Destructive`, `.Outline`, `.Ghost`, `.Link` | same names |
| `Alert` | `AlertVariant.Default` / `.Destructive` | `default` / `destructive` |
| `Avatar` | `AvatarSize.Default` / `.Sm` / `.Lg` | `default` / `sm` / `lg` |
| `Switch` | `SwitchSize.Default` / `.Sm` | `default` / `sm` |
| `Sheet` | `SheetSide.Top` / `.Right` / `.Bottom` / `.Left` | `top`/`right`/`bottom`/`left` (default `right`) |
| `Tabs` | `ShadcnOrientation.Horizontal` / `.Vertical` | `horizontal` / `vertical` |
| `TabsList` | `TabsListVariant.Default` / `.Line` | `default` / `line` |
| `DropdownMenuItem` | `DropdownMenuItemVariant.Default` / `.Destructive` | `default` / `destructive` |
| `AlertDialogContent` | `AlertDialogContentSize.Default` / `.Small` | `default` / `sm` |

## Behaviour

| Behaviour | Radix | DB.Blazor.ShadcnUI |
| --- | --- | --- |
| Dialog/Sheet/AlertDialog focus trap | yes | yes (`dp-shadcn.js`), Tab/Shift+Tab cycle, focus restored to the trigger |
| Escape closes modals | yes | yes, topmost modal only |
| Body scroll lock | yes | yes, with scrollbar-gutter compensation, shared by every open modal |
| Overlay click closes dialog | yes | yes (`CloseOnInteractOutside`, on by default; alert dialogs never) |
| Popover/dropdown outside click | yes | yes (pointerdown, trigger excluded so the toggle works) |
| Menu keyboard navigation | Arrow/Home/End, roving focus | yes (ArrowUp/Down/Home/End, Enter/Space activate) |
| Tabs keyboard navigation | Arrow/Home/End, roving tabindex | yes, including skipping disabled tabs, wrap-around |
| Accordion | single/multiple + `collapsible` | `AccordionType.Single`/`.Multiple` + `Collapsible` |
| Accordion animation | measures the content, animates `height` | same: `syncAccordion` measures `scrollHeight` into `--radix-accordion-content-height`, the CSS transitions `height` |
| Tooltip delay | provider `delayDuration` | `TooltipProvider DelayDuration` (0 by default, as in shadcn v4) |
| Tooltip stays open while hovered | yes | no — it closes when the pointer leaves the trigger (non-interactive tooltips only) |
| Submenu opens on hover | yes | yes with a 150 ms grace period, plus click |
| `aria-expanded` on the trigger | on the trigger element | on the renderless wrapper (see README) |
| Exit animations | yes | yes — closing is two-phase: the panel stays mounted with `data-state="closed"` for 150–300 ms so `animate-out` plays, then is removed |

## Known gaps (next iterations)

* Not ported: `Select`, `Sonner`, `Slider`, `Combobox`, `Command`, `Calendar`, `DatePicker`,
  `Sidebar`, `Resizable`, `Charts`, `DataTable`, `Carousel`, `Menubar`, `NavigationMenu`,
  `ContextMenu`, `HoverCard`, `Toggle`, `ToggleGroup`, `InputOTP`, `Collapsible`, `Item`, `Field`,
  `Empty`, `ButtonGroup`, `InputGroup`, `AspectRatio`, `ScrollArea`, `use-mobile`.
* `DropdownMenuItem` is a `<div role="menuitem" tabindex="-1">` with Enter/Space handled in C#;
  Radix also implements typeahead (typing to jump to an item).
* `RadioGroup` does not implement roving focus with arrow keys yet (all items are tabbable).
* Tooltips do not follow the pointer and do not stay open when the content is hovered.

## Automated parity check

`scripts/check-parity.ps1 -ReferenceDir <new-york-v4>` compares the class tokens of the shadcn/ui
registry against the ported sources in both directions:

* every utility that appears in the registry must appear somewhere in the port (a missing
  `data-[state=open]:animate-accordion-down` is a real omission, not a style choice);
* every utility a component uses must appear in at least one reference file (an unknown class is
  usually an invention).

Last run: 28 components, 0 invented classes; the only "missing" tokens left are import paths and
`data-slot` values, which the ports carry as attributes rather than as class tokens.