using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// shadcn/ui <c>Tabs</c> root: uncontrolled like Radix (<see cref="DefaultValue"/>) or controlled
/// (<see cref="Value"/> + <see cref="ValueChanged"/>, i.e. <c>@bind-Value</c>).
/// </summary>
/// <remarks>
/// Radix implements roving focus with arrow keys; the triggers here register with the root and the
/// same key handling lives in <see cref="TabsTrigger"/> — Home/End, Arrow keys, wrap-around, and
/// skipping disabled tabs.
/// </remarks>
public partial class Tabs : ShadcnComponent
{
    private readonly List<TabsTrigger> _triggers = [];
    private string? _uncontrolledValue;

    /// <summary>Controlled value; when <c>null</c> the component falls back to <see cref="DefaultValue"/>.</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Value change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Initial value for an uncontrolled tab set.</summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>Tab orientation, <c>horizontal</c> by default.</summary>
    [Parameter]
    public ShadcnOrientation Orientation { get; set; } = ShadcnOrientation.Horizontal;

    /// <summary><see cref="TabsList"/> and <see cref="TabsContent"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The currently selected value.</summary>
    public string? SelectedValue => Value ?? _uncontrolledValue ?? DefaultValue;

    /// <summary>True when <paramref name="value"/> is the selected tab.</summary>
    public bool IsActive(string? value) => string.Equals(SelectedValue, value, StringComparison.Ordinal);

    /// <summary>The DOM id of a trigger.</summary>
    public string TriggerId(string? value) => $"{ElementId}-trigger-{Sanitize(value)}";

    /// <summary>The DOM id of a panel.</summary>
    public string ContentId(string? value) => $"{ElementId}-content-{Sanitize(value)}";

    internal void Register(TabsTrigger trigger) => _triggers.Add(trigger);

    internal void Unregister(TabsTrigger trigger) => _triggers.Remove(trigger);

    internal async Task SelectAsync(string? value)
    {
        if (IsActive(value))
        {
            return;
        }

        _uncontrolledValue = value;
        await ValueChanged.InvokeAsync(value);
        StateHasChanged();
    }

    /// <summary>Moves the selection to the next enabled trigger (roving focus).</summary>
    internal async Task MoveAsync(string? current, int delta)
    {
        if (_triggers.Count == 0)
        {
            return;
        }

        var index = _triggers.FindIndex(trigger => string.Equals(trigger.Value, current, StringComparison.Ordinal));
        if (index < 0)
        {
            index = 0;
        }

        for (var step = 1; step <= _triggers.Count; step++)
        {
            var candidate = _triggers[((index + (delta * step)) % _triggers.Count + _triggers.Count) % _triggers.Count];
            if (candidate.Disabled)
            {
                continue;
            }

            await SelectAsync(candidate.Value);
            return;
        }
    }

    /// <summary>Selects the first or last enabled trigger (Home / End).</summary>
    internal async Task MoveToEdgeAsync(bool last)
    {
        var candidates = last ? Enumerable.Reverse(_triggers) : _triggers;
        foreach (var trigger in candidates)
        {
            if (!trigger.Disabled)
            {
                await SelectAsync(trigger.Value);
                return;
            }
        }
    }

    private static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "empty";
        }

        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                var c = source[i];
                span[i] = char.IsLetterOrDigit(c) ? c : '-';
            }
        });
    }
}
