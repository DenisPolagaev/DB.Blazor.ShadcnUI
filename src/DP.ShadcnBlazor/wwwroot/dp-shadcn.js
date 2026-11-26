// DP.ShadcnBlazor — the only JavaScript the components need.
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
  // Triggers are renderless wrappers (`display: contents`); the measurable box belongs to the
  // control the consumer passed as ChildContent.
  return trigger.firstElementChild ?? trigger;
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

    entry.dotnet?.invokeMethodAsync("DismissAsync");
  }
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

  const entry = {
    el,
    dotnet,
    triggerId: options.triggerId,
    reposition: () =>
      positionFloating(options.triggerId, el, {
        side: options.side,
        align: options.align,
        sideOffset: options.sideOffset,
        alignOffset: options.alignOffset,
        originVar: options.originVar,
        availableHeightVar: options.availableHeightVar,
      }),
  };

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