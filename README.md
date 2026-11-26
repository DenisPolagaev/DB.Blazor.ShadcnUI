# DP.ShadcnBlazor

> Repository **DB.Blazor.ShadcnUI** · NuGet package **DP.ShadcnBlazor** · MIT license

[shadcn/ui](https://ui.shadcn.com) ported to Blazor — Server and WebAssembly, `net10.0` and `net11.0`.

The port keeps the **component contract of shadcn/ui v4**: the same component names (`Button`,
`Card`, `CardHeader`, `Dialog`, `DropdownMenuItem`, …), the same variant values
(`default`/`destructive`/`outline`/`secondary`/`ghost`/`link`, `sm`/`lg`/`icon`), the same
`data-slot` / `data-state` / `data-variant` markup and the same Tailwind class list — only the
React/Radix plumbing is replaced by Blazor parameters, cascading contexts and one small JS module.

Authoring is one component per folder on a shared `ShadcnComponent` base (`Class`, `Style`, `Id`,
`AdditionalAttributes`), C# variant tables instead of `cva`, cascading contexts instead of React
context, and explicit two-way parameters instead of React state.

```
libs/DP.ShadcnBlazor
├─ src/DP.ShadcnBlazor          the library (Razor class library, net10.0 + net11.0)
├─ samples/DP.ShadcnBlazor.Demo Blazor Web App demo (InteractiveServer + InteractiveWebAssembly)
├─ tests/DP.ShadcnBlazor.Tests  bUnit + xUnit v3 tests (44 tests)
├─ styles/dp-shadcn.css         Tailwind v4 source of the shipped stylesheet
└─ wwwroot                      dp-shadcn.css (prebuilt) + dp-shadcn.js (the only JS)
```

## Quick start

```xml
<ProjectReference Include="libs/DP.ShadcnBlazor/src/DP.ShadcnBlazor/DP.ShadcnBlazor.csproj" />
```

`_Imports.razor`:

```razor
@using DP.ShadcnBlazor.Components
```

`index.html` / `App.razor` — one stylesheet, no `npm` required:

```html
<link rel="stylesheet" href="_content/DP.ShadcnBlazor/dp-shadcn.css" />
```

```razor
<Button OnClick="SaveAsync">Save</Button>
<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">Cancel</Button>
<Button Size="ButtonSize.Icon" aria-label="Settings"><SettingsIcon /></Button>
```

The stylesheet carries the shadcn **neutral** tokens (`--background`, `--primary`, … as
`oklch()`), the utilities the components use, and a scoped slice of Tailwind preflight. Dark mode is
the shadcn convention: add `class="dark"` to `<html>`.

To re-theme, override the tokens — no rebuild of the library is needed:

```css
:root {
  --primary: oklch(0.55 0.2 260);
  --primary-foreground: oklch(0.985 0 0);
  --radius: 0.75rem;
}
```

## What is in the first iteration

| Group | Components |
| --- | --- |
| Primitives | `Button`, `Badge`, `Card` + `CardHeader`/`CardTitle`/`CardDescription`/`CardAction`/`CardContent`/`CardFooter`, `Alert` + `AlertTitle`/`AlertDescription`, `Avatar` + `AvatarImage`/`AvatarFallback`/`AvatarBadge`/`AvatarGroup`/`AvatarGroupCount`, `Separator`, `Skeleton`, `Progress`, `Spinner`, `Kbd`/`KbdGroup` |
| Forms | `Input`, `Textarea`, `Label`, `Checkbox`, `Switch`, `RadioGroup`/`RadioGroupItem`, `NativeSelect`/`NativeSelectOption`/`NativeSelectOptGroup` |
| Data & navigation | `Table` + 8 slot components, `Tabs`/`TabsList`/`TabsTrigger`/`TabsContent`, `Accordion`/`AccordionItem`/`AccordionTrigger`/`AccordionContent`, `Breadcrumb` + 6 slots, `Pagination` + 5 slots |
| Overlays | `Dialog` (+7), `AlertDialog` (+8), `Sheet` (+6), `DropdownMenu` (+13, including checkbox items, radio items and submenus), `Popover` (+6), `Tooltip`/`TooltipProvider`/`TooltipTrigger`/`TooltipContent` |

Together that is every component a site needs to ship: layout, forms, data display, navigation,
modals and menus.

## How the React contract maps to Blazor

| shadcn/ui (React) | DP.ShadcnBlazor |
| --- | --- |
| `className` | `Class` (Blazor binds `class="…"` to it), merged with `cn()` — a port of `clsx` + `tailwind-merge` |
| `variant` / `size` props | `ButtonVariant` / `ButtonSize` enums, rendered as `data-variant` / `data-size` |
| React context (`DialogContext`, `TabsContext`, …) | `CascadingValue` of the root component |
| `asChild` + `<Slot>` | not supported. A link that looks like a button is an `<a>` with `class="@ButtonVariants.Get(...)"` — exactly shadcn's "As Link" recipe; triggers are renderless (see below) |
| Portals (`DialogPortal`, `Popover` portal) | a `data-slot="…-portal"` wrapper rendered in place; panels are `position: fixed` |
| Radix behaviour | `wwwroot/dp-shadcn.js`: focus trap, Escape, scroll lock, outside click, placement, menu keys |
| `onValueChange` / `onCheckedChange` | `ValueChanged` / `CheckedChanged`, so `@bind-Value` / `@bind-Checked` work |
| `uncontrolled` + `defaultValue` | `DefaultValue` / `DefaultOpen` (+ controlled `Value`/`Open`) |

### Triggers are renderless

React's `<DialogTrigger asChild>` clones the child and attaches the handler. Blazor cannot attach a
handler to a child's root element, so a trigger renders a wrapper with `style="display: contents"`
and forwards the event:

```razor
<Dialog>
    <DialogTrigger><Button>Edit profile</Button></DialogTrigger>
    <DialogContent>…</DialogContent>
</Dialog>
```

There is no second `<button>` (which would be invalid HTML inside a button), the markup stays
semantic, and clicks/keyboard activation of the inner control bubble to the wrapper. The trade-off:
`aria-expanded`/`aria-haspopup` live on the wrapper, not on the consumer's button.

### Deviations from shadcn/ui

1. **No portal element in `<body>`.** Panels render where the component is placed (`position: fixed`
   with a fixed z-index). A parent with `transform`, `filter` or `contain: paint` creates a
   containing block and will offset a fixed panel.
2. **Exit animations play.** Closing is two-phase: the root keeps the panel mounted with
   `data-state="closed"` for 150–300 ms (200 for dialogs, 300 for sheets) so `animate-out`,
   `fade-out-0`, `zoom-out-95` and `slide-out-to-*` run, then removes it — the lifecycle Radix
   gets from Presence.
3. **`asChild` has no equivalent.** `Button` always renders a native `<button>` (as it does in
   React without `asChild`); for a link, put `ButtonVariants.Get(...)` on an `<a>`, which is what
   the shadcn docs recommend for `<a>` elements. Triggers are the one place where the child element
   needs the handler, so they render a `display: contents` wrapper instead of cloning it.
4. **Accordion height** is measured by the same mechanism Radix uses
   (`--radix-accordion-content-height`), so the CSS transition and the class list are identical.
5. **Tooltip arrow** is a rotated square positioned by CSS on `data-side`, not an SVG arrow.
6. `Select`, `Sonner`/toasts, `Slider`, `Combobox`, `Command`, `Calendar`, `DatePicker`, `Sidebar`,
   `Resizable`, `Charts`, `DataTable`, `Carousel`, `Menubar`, `NavigationMenu`, `ContextMenu`,
   `HoverCard`, `Toggle`/`ToggleGroup`, `InputOTP`, `Collapsible`, `Item`, `Field`, `Empty`,
   `ButtonGroup`, `InputGroup`, `AspectRatio`, `ScrollArea` are **not ported yet** — they are the
   next iterations. `Sonner`, `Select` and `Slider` come first.

## Demo site

`samples/DP.ShadcnBlazor.Demo` is a copy of the ui.shadcn.com documentation site: the same header,
sidebar, "Preview / Code" cards, component pages, examples and copy, built from the ported
components. `samples/DP.ShadcnBlazor.Demo/wwwroot/docs.css` is a demo-only Tailwind bundle
(`styles/docs.css` = the library source + the demo pages), because the docs examples use utilities
(`w-56`, `max-w-sm`, `text-right`, …) that the library itself never emits.

```powershell
dotnet run --project samples/DP.ShadcnBlazor.Demo   # http://localhost:5199
```

## Styling contract

* Utilities are unprefixed: `Class="h-12 px-8"` on a component overrides its defaults
  (`tailwind-merge` semantics: `h-12` removes `h-9`, `px-8` removes `px-4`, `bg-secondary` removes
  `bg-primary`; `hover:`/`dark:`/`data-*:` variants never conflict with the base class).
* The shipped `dp-shadcn.css` is Tailwind v4 output: tokens in `oklch`, only the utilities the
  components use, **no preflight** (it would reset the host app). The slice of preflight the
  components need (`border-box`, `border: 0 solid`, form controls inheriting the page font) is
  scoped to `[data-slot]` subtrees.
* Utilities are intentionally **unlayered** so they beat that scoped base layer. Override a library
  class with your own CSS after the stylesheet, or with `Class`.
* A host that already runs Tailwind can skip `dp-shadcn.css` and point its own build at the library:
  `@source "…/libs/DP.ShadcnBlazor/src/DP.ShadcnBlazor/**/*.razor";` plus the tokens from
  `styles/dp-shadcn.css`.

## Build and test

```powershell
dotnet build DP.ShadcnBlazor.slnx        # library for net10.0 + net11.0, demo, tests
dotnet test tests/DP.ShadcnBlazor.Tests  # 44 tests (bUnit + xUnit v3)
dotnet run --project samples/DP.ShadcnBlazor.Demo   # http://localhost:5199

# Only after changing a component's class list, styles/dp-shadcn.css or a demo page:
scripts/build-css.ps1 -Install           # npm install once, then both Tailwind v4 builds
```

The generated `wwwroot/dp-shadcn.css` is committed: `dotnet build`, `dotnet pack` and every NuGet
consumer work without Node.

## License

MIT — see [LICENSE](LICENSE).

The ported class strings, markup and tokens come from [shadcn/ui](https://ui.shadcn.com) (MIT) and
the inline glyphs from [lucide](https://lucide.dev) (ISC); the full texts are in
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
