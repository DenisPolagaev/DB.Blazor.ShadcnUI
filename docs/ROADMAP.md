# Roadmap: остаток реестра shadcn/ui v4

Статус на момент последнего коммита: **46 семейств из 61** портировано и проверено.
Каждое семейство проходит один и тот же цикл, ничего не пропускается:

1. исходник: `https://ui.shadcn.com/r/styles/new-york-v4/<name>.json` → `<name>.tsx`;
2. порт с дословными классами (`Variants/*` + `Components/<Name>/*.razor`), `data-slot`/`data-state`;
3. страница в демо-доке (`Docs/*Docs.razor` + запись в `DocCatalog`);
4. `scripts/check-parity.ps1 -ReferenceDir <tsx>` — ни одного пропущенного класса, ни одного выдуманного;
5. `dotnet build DB.Blazor.ShadcnUI.slnx` (net10 + net11) и `dotnet test`;
6. коммит в `main` + обновление сабмодуля в `DP.Geoportal`.

## Готово (46)

accordion, alert, alert-dialog, aspect-ratio, avatar, badge, breadcrumb, button, button-group, card,
checkbox, collapsible, context-menu, dialog, drawer, dropdown-menu, empty, field, hover-card, input,
input-group, input-otp, item, kbd, label, menubar, native-select, pagination, popover, progress,
radio-group, scroll-area, select, separator, sheet, skeleton, slider, sonner (toaster), spinner,
switch, table, tabs, textarea, toggle, toggle-group, tooltip.

## Осталось (15)

| Семейство | Что нового относительно готового | Оценка |
|---|---|---|
| `command` | палитра: фильтрация по вводу, `CommandDialog`, хоткеи | 2 цикла |
| `combobox` | композиция `Popover` + `Command` (примитивы уже есть) | 1 цикл |
| `calendar` / `date-picker` | сетка месяцев и дат без внешних библиотек, локаль | 3–4 цикла |
| `carousel` | scroll-snap + кнопки/точки вместо embla | 2 цикла |
| `resizable` | pointer-drag панелей, новый JS-модуль | 2 цикла |
| `navigation-menu` | раскрывающиеся панели с задержками | 2–3 цикла |
| `sidebar` | раскладка + провайдер состояния + мобильный `Sheet` | 3 цикла |
| `data-table` | сортировка/фильтр/пагинация на базе `Table` | 3 цикла |
| `chart` | замена recharts: SVG area/bar/line/pie в C# | 4+ цикла |
| `typography` | только классы (`h1..h4`, `p`, `blockquote`, `list`, `inline-code`) — компонентов в реестре нет | 1 цикл |
| `attachment`, `bubble`, `message`, `message-scroller`, `marker`, `trace`, `questionnaire` | AI-элементы: у части в реестре только разметка, CSS проектируется | 1 цикл каждый |

## Анимации

Общие: `animate-in`/`animate-out`, `fade-in-0`/`fade-out-0`, `zoom-in-95`/`zoom-out-95`,
`slide-in-from-*`/`slide-out-to-*`, `animate-accordion-down/up`, `animate-caret-blink`,
`animate-spin`, `animate-pulse`, guard `prefers-reduced-motion`. Новые компоненты подключаются к
двухфазному закрытию (`IsVisible` → `data-state="closed"` → удаление через 150–300 мс) автоматически.
