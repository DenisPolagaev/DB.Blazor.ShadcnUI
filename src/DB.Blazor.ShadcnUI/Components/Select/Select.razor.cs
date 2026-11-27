using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>Select</c> root: the cascading context of a listbox select. It holds the selected
/// value and the open state, and collects the items so that <see cref="SelectValue"/> can render
/// the selected item while the list is closed.
/// </summary>
/// <remarks>
/// Radix keeps the items mounted in a detached fragment while the list is closed (their text and
/// native options must stay available); here the list is rendered with <c>hidden</c> when closed,
/// which gives the same effect without a document fragment.
/// </remarks>
public partial class Select : ShadcnFloatingRoot
{
    private readonly Dictionary<string, SelectItem> _items = new(StringComparer.Ordinal);
    private readonly Dictionary<SelectItem, string> _itemKeys = [];
    private string? _uncontrolledValue;
    private string? _notifiedValue;

    /// <summary>
    /// Raised when the selection or the item registry changes. Nested descendants
    /// (<see cref="SelectValue"/>, <see cref="SelectItem"/>) re-render from it: Blazor does not
    /// automatically refresh a component that is nested inside another component's render fragment.
    /// </summary>
    internal event Action? Changed;

    /// <summary>Controlled value (<c>@bind-Value</c>); may be <c>null</c> (nothing selected).</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Value change callback; when absent the select is uncontrolled.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Initial value of an uncontrolled select.</summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>Disables the trigger and every item.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Marks the trigger <c>aria-required</c> and the native form value required.</summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>Renders a hidden native <c>&lt;select&gt;</c> with this name so plain forms post the value.</summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>The native select's <c>autocomplete</c>.</summary>
    [Parameter]
    public string? AutoComplete { get; set; }

    /// <inheritdoc />
    protected override string SlotName => "select";

    /// <summary>The currently selected value.</summary>
    public string? SelectedValue =>
        ValueChanged.HasDelegate ? Value : _uncontrolledValue ?? DefaultValue;

    /// <summary>True when something is selected (drives <c>data-placeholder</c>).</summary>
    public bool HasValue => SelectedValue is not null;

    /// <summary>True when <paramref name="value"/> is the selected value.</summary>
    public bool IsSelected(string? value) => string.Equals(SelectedValue, value, StringComparison.Ordinal);

    /// <summary>The selected item's children, or <c>null</c> when nothing (or an unknown value) is selected.</summary>
    public RenderFragment? SelectedContent =>
        SelectedValue is not null && _items.TryGetValue(SelectedValue, out var item) ? item.ChildContent : null;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!string.Equals(_notifiedValue, SelectedValue, StringComparison.Ordinal))
        {
            _notifiedValue = SelectedValue;
            Changed?.Invoke();
        }
    }

    internal void Register(SelectItem item)
    {
        if (_itemKeys.TryGetValue(item, out var previous) && !string.Equals(previous, item.Value, StringComparison.Ordinal))
        {
            _items.Remove(previous);
        }

        _itemKeys[item] = item.Value;
        var added = !_items.ContainsKey(item.Value);
        _items[item.Value] = item;

        if (added)
        {
            // The trigger is rendered before the list, so the selected item's content appears
            // without waiting for a user interaction.
            Changed?.Invoke();
            _ = InvokeAsync(StateHasChanged);
        }
    }

    internal void Unregister(SelectItem item)
    {
        if (_itemKeys.Remove(item, out var value))
        {
            _items.Remove(value);
            Changed?.Invoke();
        }
    }

    internal async Task SelectAsync(string value)
    {
        if (IsSelected(value))
        {
            await CloseAsync();
            return;
        }

        if (ValueChanged.HasDelegate)
        {
            Value = value;
        }
        else
        {
            _uncontrolledValue = value;
        }

        _notifiedValue = value;
        Changed?.Invoke();

        await ValueChanged.InvokeAsync(value);
        await CloseAsync();
        await InvokeAsync(StateHasChanged);
    }

    private Task HandleNativeChangeAsync(ChangeEventArgs args) =>
        args.Value?.ToString() is { } value ? SelectAsync(value) : Task.CompletedTask;
}
