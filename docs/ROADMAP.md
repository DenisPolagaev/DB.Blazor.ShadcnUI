# Roadmap: остаток реестра shadcn/ui v4

Статус на момент последнего коммита: **47 семейств из 61** портировано и проверено.
Каждое семейство проходит один и тот же цикл, ничего не пропускается:

1. исходник: `https://github.com/shadcn-ui/ui` → `apps/v4/registry/bases/radix/ui/<name>.tsx`
   и стиль `apps/v4/registry/styles/style-nova.css`;
2. порт с дословными семантическими классами (`Variants/*` + `Components/<Name>/*.razor`),
   `data-slot`/`data-state`; утилиты живут в `styles/style-nova.css`, а не в C#;
3. страница в демо-доке (`Docs/*Docs.razor` + запись в `DocCatalog`);
4. `scripts/check-parity.ps1 -ReferenceDir <tsx>` — ни одного пропущенного `cn-*`;
5. `dotnet build DB.Blazor.ShadcnUI.slnx` (net10 + net11) и `dotnet test`;
6. коммит в `main` + обновление сабмодуля в `DP.Geoportal`.

## Стили

Базовый стиль — **`nova`**: компоненты несут его семантические классы (`cn-button`, `cn-card-header`,
`cn-dialog-content`, …), а `styles/style-nova.css` под `.style-nova` задаёт размеры, цвета, отступы и
состояния. Контракт `cn-*` общий у всех восьми пресетов shadcn/ui (`nova`, `luma`, `lyra`, `maia`,
`mira`, `rhea`, `sera`, `vega`), поэтому новый пресет — это только его CSS-файл, без правок
компонентов. `new-york-v4` остаётся в реестре для legacy-проектов, но этот порт его больше не
использует. Хост обязан пометить корень: `<html class="style-nova">`.

## Готово (47)

accordion, alert, alert-dialog, aspect-ratio, avatar, badge, breadcrumb, button, button-group, card,
checkbox, collapsible, command, context-menu, dialog, drawer, dropdown-menu, empty, field, hover-card, input,
input-group, input-otp, item, kbd, label, menubar, native-select, pagination, popover, progress,
radio-group, scroll-area, select, separator, sheet, skeleton, slider, sonner (toaster), spinner,
switch, table, tabs, textarea, toggle, toggle-group, tooltip.

## Осталось (14)

| Семейство | Что нового относительно готового | Оценка |
|---|---|---|
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
