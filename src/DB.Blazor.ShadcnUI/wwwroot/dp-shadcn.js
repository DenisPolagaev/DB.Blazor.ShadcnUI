// DB.Blazor.ShadcnUI — the only JavaScript the components need.
//
// Everything else is CSS and Blazor. This module owns the four behaviours a component model
// without portals cannot express on its own:
//   1. modal dialogs   — focus trap, Escape, scroll lock, focus restore;
//   2. dismissable panels (popover / dropdown menu) — outside click, Escape, menu key navigation;
//   3. floating placement — fixed positioning with flip + viewport clamping, `data-side`
//      for the slide-in animations, and the `--radix-*-transform-origin` variable the shadcn
//      class list consumes;
//   4. scroll locking shared by every open modal.
//
// There is no global namespace and no dependency: the file is imported as an ES module.

const FOCUSABLE = [
  "a[href]",
  "area[href]",
  "button:not([disabled])",
  "input:not([disabled]):not([type='hidden'])",
  "select:not([disabled])",
  "textarea:not([disabled])",
  "iframe",
  "[contenteditable='true']",
  "[tabindex]:not([tabindex='-1'])",
].join(",");

const modalStack = [];
const dismissables = new Set();
let scrollLocks = 0;
let previousOverflow = "";
let previousPaddingRight = "";

function focusables(root) {
  return Array.from(root.querySelectorAll(FOCUSABLE)).filter(
    (el) => !el.hasAttribute("inert") && el.getClientRects().length > 0,
  );
}

function focusFirst(root) {
  const target = focusables(root)[0] ?? root;
  target.focus?.({ preventScroll: true });
}

function lockScroll() {
  if (scrollLocks === 0) {
    const doc = document.documentElement;
    const gap = window.innerWidth - doc.clientWidth;
    previousOverflow = doc.style.overflow;
    previousPaddingRight = doc.style.paddingRight;
    doc.style.overflow = "hidden";
    if (gap > 0) {
      doc.style.paddingRight = `${gap}px`;
    }
  }
  scrollLocks += 1;
}

function unlockScroll() {
  scrollLocks = Math.max(0, scrollLocks - 1);
  if (scrollLocks === 0) {
    const doc = document.documentElement;
    doc.style.overflow = previousOverflow;
    doc.style.paddingRight = previousPaddingRight;
  }
}

function topModal() {
  return modalStack.length > 0 ? modalStack[modalStack.length - 1] : null;
}

function onModalKeyDown(event) {
  const modal = topModal();
  if (!modal) {
    return;
  }

  if (event.key === "Escape") {
    event.preventDefault();
    modal.dotnet?.invokeMethodAsync("DismissAsync");
    return;
  }

  if (event.key !== "Tab") {
    return;
  }

  const items = focusables(modal.el);
  if (items.length === 0) {
    event.preventDefault();
    modal.el.focus?.({ preventScroll: true });
    return;
  }

  const first = items[0];
  const last = items[items.length - 1];
  const active = document.activeElement;

  if (!modal.el.contains(active)) {
    event.preventDefault();
    (event.shiftKey ? last : first).focus();
    return;
  }

  if (event.shiftKey && active === first) {
    event.preventDefault();
    last.focus();
  } else if (!event.shiftKey && active === last) {
    event.preventDefault();
    first.focus();
  }
}

/** Opens a modal panel: focus trap, Escape, scroll lock, focus restore on close. */
export function openModal(el, dotnet) {
  if (!el) {
    return;
  }

  modalStack.push({ el, dotnet, previous: document.activeElement });
  el.dataset.state = "open";
  lockScroll();
  document.addEventListener("keydown", onModalKeyDown, true);
  requestAnimationFrame(() => focusFirst(el));
}

/** Closes a modal panel and restores focus to the element that opened it. `id` is the panel's DOM
 *  id: the element may already be gone by the time Blazor detaches it, so the stack is matched by
 *  id (or, without one, by the topmost entry). */
export function closeModal(id) {
  let index = typeof id === "string" ? modalStack.findIndex((entry) => entry.el.id === id) : -1;
  if (index < 0) {
    index = modalStack.length - 1;
  }
  if (index < 0) {
    return;
  }

  const [entry] = modalStack.splice(index, 1);
  entry.el.dataset.state = "closed";
  if (modalStack.length === 0) {
    document.removeEventListener("keydown", onModalKeyDown, true);
    unlockScroll();
  }

  entry.previous?.focus?.({ preventScroll: true });
}

function triggerElement(triggerId) {
  const trigger = triggerId ? document.getElementById(triggerId) : null;
  if (!trigger) {
    return null;
  }
  // Menu triggers are renderless wrappers (`display: contents`); the measurable box belongs to the
  // control the consumer passed as ChildContent. A select trigger is the real <button>.
  if (trigger.style.display === "contents" || getComputedStyle(trigger).display === "contents") {
    return trigger.firstElementChild ?? trigger;
  }
  return trigger;
}

/**
 * Places a floating panel next to its trigger.
 * `originVar` is the CSS variable the shadcn class list reads (e.g.
 * `--radix-popover-content-transform-origin`), so `origin-(--radix-…)` keeps working.
 */
export function positionFloating(triggerId, panel, options = {}) {
  const anchor = triggerElement(triggerId);
  if (!anchor || !panel) {
    return;
  }

  const side = options.side ?? "bottom";
  const align = options.align ?? "center";
  const sideOffset = options.sideOffset ?? 4;
  const alignOffset = options.alignOffset ?? 0;
  const padding = 8;

  panel.style.position = "fixed";
  panel.style.visibility = "hidden";
  panel.style.left = "0px";
  panel.style.top = "0px";

  const rect = anchor.getBoundingClientRect();
  const size = panel.getBoundingClientRect();

  let resolved = side;
  const spaceTop = rect.top;
  const spaceBottom = window.innerHeight - rect.bottom;
  if (side === "bottom" && spaceBottom < size.height + sideOffset + padding && spaceTop > spaceBottom) {
    resolved = "top";
  } else if (side === "top" && spaceTop < size.height + sideOffset + padding && spaceBottom > spaceTop) {
    resolved = "bottom";
  }

  let top = resolved === "bottom" ? rect.bottom + sideOffset : rect.top - size.height - sideOffset;
  let left;
  if (align === "start") {
    left = rect.left + alignOffset;
  } else if (align === "end") {
    left = rect.right - size.width - alignOffset;
  } else {
    left = rect.left + (rect.width - size.width) / 2 + alignOffset;
  }

  left = Math.min(Math.max(padding, left), window.innerWidth - size.width - padding);
  top = Math.min(Math.max(padding, top), window.innerHeight - size.height - padding);

  panel.style.left = `${Math.round(left)}px`;
  panel.style.top = `${Math.round(top)}px`;
  panel.style.visibility = "";
  panel.dataset.side = resolved;

  if (options.originVar) {
    const originX = align === "start" ? 0 : align === "end" ? 100 : 50;
    const originY = resolved === "top" ? 100 : 0;
    panel.style.setProperty(options.originVar, `${originX}% ${originY}%`);
  }

  if (options.availableHeightVar) {
    panel.style.setProperty(options.availableHeightVar, `${Math.max(0, window.innerHeight - top - padding)}px`);
  }
}

/**
 * Places a select listbox. Radix has two modes: `popper` behaves like any floating panel, and
 * `item-aligned` (the shadcn default) overlays the list on the trigger so the checked item sits
 * where the trigger is. Both expose the `--radix-select-*` variables the shadcn class list reads.
 */
export function positionSelect(triggerId, panel, options = {}) {
  const anchor = triggerElement(triggerId);
  if (!anchor || !panel) {
    return;
  }

  const padding = 8;

  // Take the panel out of flow before measuring: it renders in place (no portal), and its
  // `relative` class would otherwise shift the trigger for one layout pass.
  panel.style.position = "fixed";
  panel.style.visibility = "hidden";
  panel.style.left = "0px";
  panel.style.top = "0px";

  const triggerRect = anchor.getBoundingClientRect();
  // Measure the natural height: the previous open may have left the available-height limit set.
  panel.style.removeProperty("--radix-select-content-available-height");

  if (options.itemAligned) {
    const panelRect = panel.getBoundingClientRect();
    const checked = panel.querySelector('[role="option"][data-state="checked"]');
    const textNode = checked?.lastElementChild;
    const itemTextRect = textNode?.getBoundingClientRect();
    const triggerPaddingLeft = Number.parseFloat(getComputedStyle(anchor).paddingLeft) || 0;

    // Radix lines the selected item's text up with the trigger's value text, not just the edges.
    const itemTextOffset = itemTextRect ? itemTextRect.left - panelRect.left : 0;
    const left = triggerRect.left + triggerPaddingLeft - itemTextOffset;
    const leftDelta = triggerRect.left - left;
    panel.style.minWidth = `${Math.ceil(triggerRect.width + leftDelta)}px`;

    const size = panel.getBoundingClientRect();
    const itemRect = checked ? checked.getBoundingClientRect() : null;
    const itemOffset = itemRect ? itemRect.top - size.top : 0;
    const top = Math.min(
      Math.max(padding, triggerRect.top - itemOffset),
      Math.max(padding, window.innerHeight - size.height - padding),
    );
    const clampedLeft = Math.min(
      Math.max(padding, left),
      Math.max(padding, window.innerWidth - size.width - padding),
    );

    panel.style.left = `${Math.round(clampedLeft)}px`;
    panel.style.top = `${Math.round(top)}px`;
    panel.style.setProperty(
      "--radix-select-content-available-height",
      `${Math.max(0, window.innerHeight - top - padding)}px`,
    );
    panel.style.setProperty("--radix-select-content-transform-origin", "center top");
    panel.dataset.side = "bottom";
  } else {
    positionFloating(triggerId, panel, {
      side: "bottom",
      align: options.align ?? "center",
      sideOffset: options.sideOffset ?? 4,
      originVar: "--radix-select-content-transform-origin",
      availableHeightVar: "--radix-select-content-available-height",
    });
  }

  panel.style.setProperty("--radix-select-trigger-width", `${Math.ceil(triggerRect.width)}px`);
  panel.style.setProperty("--radix-select-trigger-height", `${Math.ceil(triggerRect.height)}px`);
  panel.style.visibility = "";

  // A scroll button appearing shifts the viewport, which breaks the item alignment: position once
  // more with the final button visibility.
  if (syncSelectScrollButtons(panel) && options.itemAligned) {
    positionSelect(triggerId, panel, options);
  }
}

/** Radix mounts a scroll button only while the list can scroll in that direction. */
function syncSelectScrollButtons(panel) {
  const up = panel.querySelector('[data-slot="select-scroll-up-button"]');
  const down = panel.querySelector('[data-slot="select-scroll-down-button"]');
  let changed = false;
  if (up) {
    const visible = panel.scrollTop > 0;
    changed ||= (up.style.display !== "none") !== visible;
    // The class list carries `flex`, which wins over the `hidden` attribute: set display.
    up.style.display = visible ? "" : "none";
  }
  if (down) {
    const visible = panel.scrollTop + panel.clientHeight < panel.scrollHeight - 1;
    changed ||= (down.style.display !== "none") !== visible;
    down.style.display = visible ? "" : "none";
  }
  return changed;
}

const triggerTypeahead = new WeakMap();

/**
 * Closed-select keyboard behaviour: Arrow keys open the list, typing picks an item without
 * opening it (Radix's typeahead), both without scrolling the page.
 */
function onSelectTriggerKeyDown(event) {
  if (event.key === "ArrowDown" || event.key === "ArrowUp") {
    event.preventDefault();
    if (event.currentTarget.getAttribute("aria-expanded") !== "true") {
      event.currentTarget.click();
    }
    return;
  }

  if (event.key.length !== 1 || event.ctrlKey || event.metaKey || event.altKey) {
    return;
  }

  const trigger = event.currentTarget;
  const content = document.getElementById(trigger.getAttribute("aria-controls"));
  if (!content) {
    return;
  }

  const items = Array.from(content.querySelectorAll('[role="option"]')).filter(
    (item) => !item.hasAttribute("data-disabled") && item.getAttribute("aria-disabled") !== "true",
  );
  if (items.length === 0) {
    return;
  }

  const state = triggerTypeahead.get(trigger) ?? { text: "", at: 0 };
  const now = Date.now();
  // A repeated character cycles through the values that start with it.
  state.text =
    now - state.at > 500 || state.text === event.key ? event.key : state.text + event.key;
  state.at = now;
  triggerTypeahead.set(trigger, state);

  const search = state.text.toLowerCase();
  const checked = items.find((item) => item.getAttribute("data-state") === "checked");
  const start = items.indexOf(checked);
  const ordered = [...items.slice(start + 1), ...items.slice(0, start + 1)];
  const match = ordered.find((item) => (item.textContent ?? "").trim().toLowerCase().startsWith(search));
  match?.click();
}

/** Attaches the closed-select keyboard behaviour to a trigger button. */
export function bindSelectTrigger(el) {
  if (!el || el.dataset.selectTriggerBound === "true") {
    return;
  }
  el.dataset.selectTriggerBound = "true";
  el.addEventListener("keydown", onSelectTriggerKeyDown);
}

function closestDismissable(target) {
  for (const entry of dismissables) {
    if (entry.el.contains(target)) {
      return entry;
    }
  }
  return null;
}

function onDismissablePointerDown(event) {
  for (const entry of Array.from(dismissables)) {
    if (entry.el.contains(event.target)) {
      continue;
    }

    const trigger = entry.triggerId ? document.getElementById(entry.triggerId) : null;
    if (trigger && trigger.contains(event.target)) {
      continue;
    }

    entry.dismissedByPointer = true;
    entry.dotnet?.invokeMethodAsync("DismissAsync");
  }
}

/** Arrow/Home/End, Enter/Space and typeahead over a select listbox. */
function handleListboxKeyDown(entry, event) {
  const panel = entry.el;
  const items = Array.from(panel.querySelectorAll('[role="option"]')).filter(
    (item) => !item.hasAttribute("data-disabled") && item.getAttribute("aria-disabled") !== "true",
  );
  if (items.length === 0) {
    return;
  }

  const active = document.activeElement;
  const index = items.indexOf(active);
  let next = -1;

  switch (event.key) {
    case "ArrowDown":
      next = index + 1;
      break;
    case "ArrowUp":
      next = index <= 0 ? items.length - 1 : index - 1;
      break;
    case "Home":
      next = 0;
      break;
    case "End":
      next = items.length - 1;
      break;
    case "Enter":
    case " ":
      event.preventDefault();
      (index >= 0 ? items[index] : items[0]).click();
      return;
    case "Tab":
      // Radix keeps focus inside an open select listbox; Escape is the way out.
      event.preventDefault();
      return;
    default:
      if (event.key.length === 1 && !event.ctrlKey && !event.metaKey && !event.altKey) {
        event.preventDefault();
        typeaheadSelect(entry, items, event.key);
      }
      return;
  }

  event.preventDefault();
  const item = items[(next + items.length) % items.length];
  item?.focus({ preventScroll: true });
  item?.scrollIntoView({ block: "nearest" });
}

function typeaheadSelect(entry, items, key) {
  const now = Date.now();
  if (!entry.typeahead || now - entry.typeahead.at > 500) {
    entry.typeahead = { text: "", at: now };
  }
  // Repeated characters cycle through the items that start with that character.
  entry.typeahead.text = entry.typeahead.text === key ? key : entry.typeahead.text + key;
  entry.typeahead.at = now;

  const search = entry.typeahead.text.toLowerCase();
  const start = items.indexOf(document.activeElement);
  const ordered = [...items.slice(start + 1), ...items.slice(0, start + 1)];
  const match = ordered.find((item) => (item.textContent ?? "").trim().toLowerCase().startsWith(search));
  match?.focus({ preventScroll: true });
  match?.scrollIntoView({ block: "nearest" });
}

function onDismissableKeyDown(event) {
  if (dismissables.size === 0) {
    return;
  }

  const entries = Array.from(dismissables);
  const entry = entries[entries.length - 1];
  const panel = entry.el;

  if (event.key === "Escape") {
    event.preventDefault();
    entry.dotnet?.invokeMethodAsync("DismissAsync");
    return;
  }

  if (!panel.contains(document.activeElement) && !panel.contains(event.target)) {
    return;
  }

  if (panel.getAttribute("role") === "listbox") {
    handleListboxKeyDown(entry, event);
    return;
  }

  if (!panel.matches('[data-slot="dropdown-menu-content"],[data-slot="dropdown-menu-sub-content"]')) {
    return;
  }

  const items = Array.from(
    panel.querySelectorAll('[role="menuitem"],[role="menuitemcheckbox"],[role="menuitemradio"]'),
  ).filter((item) => !item.hasAttribute("data-disabled") && !item.disabled);

  if (items.length === 0) {
    return;
  }

  const index = items.indexOf(document.activeElement);
  let next = -1;

  switch (event.key) {
    case "ArrowDown":
      next = index + 1;
      break;
    case "ArrowUp":
      next = index <= 0 ? items.length - 1 : index - 1;
      break;
    case "Home":
      next = 0;
      break;
    case "End":
      next = items.length - 1;
      break;
    default:
      return;
  }

  event.preventDefault();
  items[(next + items.length) % items.length]?.focus();
}

/**
 * Opens a non-modal floating panel (popover, dropdown menu).
 * Escape and an outside pointerdown dismiss it; menus additionally get Arrow/Home/End navigation.
 */
export function openDismissable(el, dotnet, options = {}) {
  if (!el) {
    return;
  }

  const listbox = options.listbox === true;
  const entry = {
    el,
    dotnet,
    triggerId: options.triggerId,
    reposition: () =>
      listbox
        ? positionSelect(options.triggerId, el, {
            itemAligned: options.itemAligned,
            align: options.align,
            sideOffset: options.sideOffset,
          })
        : positionFloating(options.triggerId, el, {
            side: options.side,
            align: options.align,
            sideOffset: options.sideOffset,
            alignOffset: options.alignOffset,
            originVar: options.originVar,
            availableHeightVar: options.availableHeightVar,
          }),
  };

  if (listbox) {
    entry.onScroll = () => syncSelectScrollButtons(el);
    entry.onClick = (event) => {
      const button = event.target.closest?.(
        '[data-slot="select-scroll-up-button"],[data-slot="select-scroll-down-button"]',
      );
      if (!button) {
        return;
      }
      event.preventDefault();
      const direction = button.dataset.slot === "select-scroll-up-button" ? -1 : 1;
      el.scrollBy({ top: direction * el.clientHeight * 0.8 });
    };
    el.addEventListener("scroll", entry.onScroll, true);
    el.addEventListener("click", entry.onClick);
  }

  dismissables.add(entry);
  el.dataset.state = "open";
  document.addEventListener("pointerdown", onDismissablePointerDown, true);
  document.addEventListener("keydown", onDismissableKeyDown, true);
  window.addEventListener("resize", entry.reposition);
  window.addEventListener("scroll", entry.reposition, true);

  requestAnimationFrame(() => {
    entry.reposition();
    if (options.focus === false) {
      return;
    }

    if (options.focus === "panel") {
      el.focus?.({ preventScroll: true });
      return;
    }

    // Radix focuses the checked option of a select, or the first valid one.
    if (listbox) {
      const items = Array.from(el.querySelectorAll('[role="option"]')).filter(
        (item) => !item.hasAttribute("data-disabled") && item.getAttribute("aria-disabled") !== "true",
      );
      const item =
        items.find((candidate) => candidate.getAttribute("data-state") === "checked") ?? items[0];
      if (item) {
        item.focus({ preventScroll: true });
        item.scrollIntoView({ block: "nearest" });
        return;
      }
    }

    // Radix focuses the first menu item, not the menu container: the container has no outline
    // style in the shadcn class list, so focusing it would paint the browser's focus ring.
    if (el.matches('[role="menu"]')) {
      const item = el.querySelector(
        '[role="menuitem"]:not([data-disabled]),[role="menuitemcheckbox"]:not([data-disabled]),[role="menuitemradio"]:not([data-disabled])',
      );
      if (item) {
        item.focus({ preventScroll: true });
        return;
      }
    }

    focusFirst(el);
  });
}

/** Closes a dismissable panel. Safe to call twice (Dispose + outside click can race). `id` matches
 *  the panel that may already have left the DOM. */
export function closeDismissable(id) {
  const matches = (entry) =>
    (typeof id === "string" && entry.el.id === id) || (id === undefined && dismissables.size > 0);

  const targets = Array.from(dismissables).filter(matches);
  if (typeof id === "string" && targets.length === 0) {
    return;
  }

  for (const entry of targets) {
    dismissables.delete(entry);
    entry.el.dataset.state = "closed";
    window.removeEventListener("resize", entry.reposition);
    window.removeEventListener("scroll", entry.reposition, true);
    if (entry.onScroll) {
      entry.el.removeEventListener("scroll", entry.onScroll, true);
    }
    if (entry.onClick) {
      entry.el.removeEventListener("click", entry.onClick);
    }
    // Radix returns focus to the trigger after Escape or a selection; after an outside pointer
    // press the user's click decides where focus goes.
    if (!entry.dismissedByPointer) {
      triggerElement(entry.triggerId)?.focus?.({ preventScroll: true });
    }
    if (dismissables.size === 0) {
      document.removeEventListener("pointerdown", onDismissablePointerDown, true);
      document.removeEventListener("keydown", onDismissableKeyDown, true);
    }
  }
}

/** Moves focus into a panel (used by popovers that contain no focusable element). */
export function focusPanel(el) {
  el?.focus?.({ preventScroll: true });
}

const commandBindings = new WeakMap();

/**
 * Command palette behaviour (cmdk): client-side filtering, the highlighted item, arrow/Home/End
 * navigation, Enter to choose, and the empty state. The components are static markup; everything
 * that changes on every keystroke stays in the browser.
 */
export function bindCommand(el) {
  if (!el || commandBindings.has(el)) {
    return;
  }

  const state = { input: null, items: [], groups: [], empty: null };
  commandBindings.set(el, state);

  state.onInput = () => applyCommandFilter(el);
  state.onKeyDown = (event) => handleCommandKeyDown(el, event);
  state.onPointerMove = (event) => {
    const item = event.target.closest?.("[cmdk-item]");
    if (!item || !el.contains(item)) {
      return;
    }
    selectCommandItem(el, item);
  };

  el.addEventListener("input", state.onInput);
  el.addEventListener("keydown", state.onKeyDown);
  el.addEventListener("pointermove", state.onPointerMove);

  applyCommandFilter(el);
}

function commandParts(el) {
  return {
    input: el.querySelector("[cmdk-input]"),
    items: Array.from(el.querySelectorAll("[cmdk-item]")),
    groups: Array.from(el.querySelectorAll("[cmdk-group]")),
    empty: el.querySelector("[cmdk-empty]"),
  };
}

function setCommandVisible(element, visible) {
  const display = visible ? "" : "none";
  if (element.style.display !== display) {
    element.style.display = display;
  }
  if (element.hidden === visible) {
    element.hidden = !visible;
  }
}

function commandItemValue(item) {
  const value = item.getAttribute("data-value");
  return (value ?? item.textContent ?? "").trim().toLowerCase();
}

function applyCommandFilter(el) {
  const { input, items, groups, empty } = commandParts(el);
  const search = (input?.value ?? "").trim().toLowerCase();
  const filtering = el.dataset.shouldFilter !== "false" && search.length > 0;

  for (const item of items) {
    const value = commandItemValue(item);
    const keywords = (item.getAttribute("data-keywords") ?? "").toLowerCase();
    const matches = !filtering || value.includes(search) || (keywords.length > 0 && keywords.includes(search));
    setCommandVisible(item, matches);
    if (!matches) {
      item.setAttribute("aria-selected", "false");
      item.dataset.selected = "false";
    }
  }

  for (const group of groups) {
    const hasVisibleItem = Array.from(group.querySelectorAll("[cmdk-item]")).some(
      (item) => item.style.display !== "none",
    );
    setCommandVisible(group, hasVisibleItem);
  }

  if (empty) {
    setCommandVisible(empty, !items.some((item) => item.style.display !== "none"));
  }

  // cmdk always highlights the first enabled item after the filter changes.
  const enabled = enabledCommandItems(el);
  selectCommandItem(el, enabled[0] ?? null);
}

function enabledCommandItems(el) {
  return Array.from(el.querySelectorAll("[cmdk-item]")).filter(
    (item) =>
      item.style.display !== "none" &&
      item.getAttribute("data-disabled") !== "true" &&
      item.getAttribute("aria-disabled") !== "true",
  );
}

function selectedCommandItem(el) {
  return el.querySelector('[cmdk-item][data-selected="true"]');
}

function selectCommandItem(el, item) {
  if (
    item &&
    (item.getAttribute("data-disabled") === "true" || item.getAttribute("aria-disabled") === "true")
  ) {
    return;
  }

  const previous = selectedCommandItem(el);
  if (previous && previous !== item) {
    previous.dataset.selected = "false";
    previous.setAttribute("aria-selected", "false");
  }

  const input = el.querySelector("[cmdk-input]");
  if (!item) {
    input?.removeAttribute("aria-activedescendant");
    return;
  }

  item.dataset.selected = "true";
  item.setAttribute("aria-selected", "true");
  if (input && item.id) {
    input.setAttribute("aria-activedescendant", item.id);
  }
  item.scrollIntoView({ block: "nearest" });
}

function handleCommandKeyDown(el, event) {
  if (event.key === "Enter") {
    const selected = selectedCommandItem(el);
    const items = enabledCommandItems(el);
    const target = selected ?? items[0];
    if (target) {
      event.preventDefault();
      target.click();
    }
    return;
  }

  const items = enabledCommandItems(el);
  if (items.length === 0) {
    return;
  }

  const current = items.indexOf(selectedCommandItem(el));
  const loop = el.dataset.loop === "true";
  let next;

  switch (event.key) {
    case "ArrowDown":
      next = current < 0 ? 0 : current + 1;
      break;
    case "ArrowUp":
      next = current < 0 ? items.length - 1 : current - 1;
      break;
    case "Home":
      next = 0;
      break;
    case "End":
      next = items.length - 1;
      break;
    case "PageDown":
      next = Math.min(items.length - 1, current + 8);
      break;
    case "PageUp":
      next = Math.max(0, current - 8);
      break;
    default:
      return;
  }

  event.preventDefault();
  if (next < 0) {
    next = loop ? items.length - 1 : 0;
  } else if (next >= items.length) {
    next = loop ? 0 : items.length - 1;
  }

  selectCommandItem(el, items[next]);
}

/**
 * Sizes an accordion panel. Radix measures the content and exposes the height as
 * `--radix-accordion-content-height`; doing the same keeps the CSS (and therefore the class
 * list) identical to shadcn/ui, and the height transition runs on the real content height.
 */
export function syncAccordion(el, open) {
  if (!el) {
    return;
  }

  const height = el.scrollHeight;
  el.style.setProperty("--radix-accordion-content-height", `${height}px`);
  el.style.height = open ? `${height}px` : "0px";
}


