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
  `sheet-description`, `select`, `select-group`, `select-value`, `select-trigger`, `select-content`,
  `select-label`, `select-item`, `select-item-indicator`, `select-separator`,
  `select-scroll-up-button`, `select-scroll-down-button`, `command`, `command-input-wrapper`,
  `command-input`, `command-list`, `command-empty`, `command-group`, `command-item`,
  `command-separator`, `command-shortcut`,
  `dropdown-menu-content`, `dropdown-menu-item`, `dropdown-menu-checkbox-item`,
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
* `SelectContent` stays mounted with the `hidden` attribute while the select is closed. Radix keeps
  the items in a detached document fragment for the same reason: `SelectValue` and the hidden
  native `<select>` need the item text and values before the list has ever been opened.
* `Command` shares its name with the deprecated HTML void element `<command>`, so Razor requires the
  fully qualified root tag `<DB.Blazor.ShadcnUI.Components.Command>`; the other `Command*` components
  need no prefix.
* Command filtering is case-insensitive substring matching; cmdk ranks with its `command-score` fuzzy
  matcher, so ordering of partial matches can differ.

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
| Select listbox | Arrow/Home/End, typeahead, selected item focused on open, Escape/Tab handling | yes (the same keys plus closed-trigger typeahead; Tab is kept inside the open list like Radix) |
| Select placement | `item-aligned` (default) or `popper` | both; item-aligned overlays the list on the trigger so the checked item lines up with it |
| Command filtering | cmdk filters items by value/keywords as you type, hides empty groups and shows `CommandEmpty` | same, client-side in `dp-shadcn.js` (`data-value`/`data-keywords`, falling back to the item text) |
| Command keyboard | Arrow/Home/End/PageUp/PageDown, Enter chooses | same; `Loop` wraps the selection |
| CommandDialog hotkey | not part of cmdk: the docs example binds a key itself | open it from your own key handler with `@bind-Open`; the component registers no global listener |
| Tabs keyboard navigation | Arrow/Home/End, roving tabindex | yes, including skipping disabled tabs, wrap-around |
| Accordion | single/multiple + `collapsible` | `AccordionType.Single`/`.Multiple` + `Collapsible` |
| Accordion animation | measures the content, animates `height` | same: `syncAccordion` measures `scrollHeight` into `--radix-accordion-content-height`, the CSS transitions `height` |
| Tooltip delay | provider `delayDuration` | `TooltipProvider DelayDuration` (0 by default, as in shadcn v4) |
| Tooltip stays open while hovered | yes | no — it closes when the pointer leaves the trigger (non-interactive tooltips only) |
| Submenu opens on hover | yes | yes with a 150 ms grace period, plus click |
| `aria-expanded` on the trigger | on the trigger element | on the renderless wrapper (see README) |
| Exit animations | yes | yes — closing is two-phase: the panel stays mounted with `data-state="closed"` for 150–300 ms so `animate-out` plays, then is removed |

## Known gaps (next iterations)

* Not ported: `Combobox`, `Calendar`, `DatePicker`, `Carousel`, `Resizable`, `NavigationMenu`,
  `Sidebar`, `DataTable`, `Charts`, `Typography` and the AI elements (`Attachment`, `Bubble`,
  `Message`, `MessageScroller`, `Marker`, `Trace`, `Questionnaire`), plus `use-mobile`.
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

Last run: 46 components, 0 invented classes; the only "missing" tokens left are import paths,
`data-slot` values and Radix's `item-aligned` literal, which the ports carry as attributes or
parameters rather than as class tokens.