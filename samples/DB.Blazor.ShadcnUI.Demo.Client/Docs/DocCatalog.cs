namespace DB.Blazor.ShadcnUI.Demo.Client.Docs;

/// <summary>One entry of the docs sidebar: the same order and names as shadcn/ui's component list.</summary>
public sealed record DocEntry(string Slug, string Name, string Description);

/// <summary>
/// The catalogue the sidebar and the component index are generated from — only components that are
/// actually ported, so the site never links to a page that does not exist.
/// </summary>
public static class DocCatalog
{
    public static readonly IReadOnlyList<DocEntry> Components =
    [
        new("aspect-ratio", "Aspect Ratio", "Displays content within a desired ratio."),
        new("accordion", "Accordion", "A vertically stacked set of interactive headings that each reveal a section of content."),
        new("alert", "Alert", "Displays a callout for user attention."),
        new("alert-dialog", "Alert Dialog", "A modal dialog that interrupts the user with important content and expects a response."),
        new("avatar", "Avatar", "An image element with a fallback for representing the user."),
        new("badge", "Badge", "Displays a badge or a component that looks like a badge."),
        new("breadcrumb", "Breadcrumb", "Displays the path to the current resource using a hierarchy of links."),
        new("button-group", "Button Group", "A container that groups related buttons together with consistent styling."),
        new("button", "Button", "Displays a button or a component that looks like a button."),
        new("card", "Card", "Displays a card with header, content, and footer."),
        new("collapsible", "Collapsible", "An interactive component which expands/collapses a panel."),
        new("checkbox", "Checkbox", "A control that allows the user to toggle between checked and not checked."),
        new("dialog", "Dialog", "A window overlaid on either the primary window or another dialog window."),
        new("dropdown-menu", "Dropdown Menu", "Displays a menu to the user — such as a set of actions or functions — triggered by a button."),
        new("drawer", "Drawer", "A drawer component for React."),
        new("empty", "Empty", "Use the Empty component to display an empty state."),
        new("field", "Field", "Combine labels, controls, and help text to compose accessible form fields."),
        new("hover-card", "Hover Card", "For sighted users to preview content available behind a link."),
        new("input-otp", "Input OTP", "Accessible one-time password component with copy paste functionality."),
        new("input-group", "Input Group", "A set of composable input components for building rich form controls."),
        new("input", "Input", "Displays a form input field or a component that looks like an input field."),
        new("item", "Item", "A flexible component for displaying an item in a container."),
        new("kbd", "Kbd", "Used to display keyboard keys or shortcuts."),
        new("label", "Label", "Renders an accessible label associated with controls."),
        new("native-select", "Native Select", "A native select element with a custom chevron and consistent styling."),
        new("pagination", "Pagination", "Pagination with page navigation, next and previous links."),
        new("popover", "Popover", "Displays rich content in a portal, triggered by a button."),
        new("progress", "Progress", "Displays an indicator showing the completion progress of a task."),
        new("radio-group", "Radio Group", "A set of checkable buttons — known as radio buttons — where no more than one can be checked at a time."),
        new("scroll-area", "Scroll Area", "Augments native scroll functionality for custom, cross-browser styling."),
        new("separator", "Separator", "Visually or semantically separates content."),
        new("sheet", "Sheet", "Extends the Dialog component to display content that complements the main content of the screen."),
        new("skeleton", "Skeleton", "Use to show a placeholder while content is loading."),
        new("slider", "Slider", "An input where the user selects a value from within a given range."),
        new("spinner", "Spinner", "An indicator that can be used to show a loading state."),
        new("switch", "Switch", "A control that allows the user to toggle between checked and not checked."),
        new("table", "Table", "A responsive table component."),
        new("tabs", "Tabs", "A set of layered sections of content — known as tab panels — that are displayed one at a time."),
        new("textarea", "Textarea", "Displays a form textarea or a component that looks like a textarea."),
        new("toggle", "Toggle", "A two-state button that can be either on or off."),
        new("toggle-group", "Toggle Group", "A set of two-state buttons that can be toggled on or off."),
        new("tooltip", "Tooltip", "A popup that displays information related to an element when the element receives keyboard focus or the mouse hovers over it."),
    ];

    public static DocEntry? Find(string? slug) =>
        Components.FirstOrDefault(entry => string.Equals(entry.Slug, slug, StringComparison.OrdinalIgnoreCase));
}
