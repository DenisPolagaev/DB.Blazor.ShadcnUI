using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// Radix's <c>ToggleGroup type</c>: <c>single</c> keeps one value, <c>multiple</c> a list.
/// </summary>
public partial class ToggleGroup : ShadcnComponent
{
    private readonly List<string> _uncontrolledValues = [];
    private string? _uncontrolledValue;

    /// <summary><c>single</c> (default) or <c>multiple</c>.</summary>
    [Parameter]
    public ToggleGroupType Type { get; set; } = ToggleGroupType.Single;

    /// <summary>Controlled value for a single group.</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Single value change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Controlled values for a multiple group.</summary>
    [Parameter]
    public IReadOnlyList<string>? Values { get; set; }

    /// <summary>Multiple values change callback.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<string>> ValuesChanged { get; set; }

    /// <summary>Item variant, inherited by the items.</summary>
    [Parameter]
    public ToggleVariant Variant { get; set; } = ToggleVariant.Default;

    /// <summary>Item size, inherited by the items.</summary>
    [Parameter]
    public ToggleSize Size { get; set; } = ToggleSize.Default;

    /// <summary>Gap between the items (Radix's <c>spacing</c>, 0 by default).</summary>
    [Parameter]
    public int Spacing { get; set; }

    /// <summary>Disables every item.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary><see cref="ToggleGroupItem"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when <paramref name="value"/> is pressed.</summary>
    public bool IsPressed(string? value) => Type == ToggleGroupType.Multiple
        ? (Values ?? _uncontrolledValues).Contains(value ?? string.Empty, StringComparer.Ordinal)
        : string.Equals(Value ?? _uncontrolledValue, value, StringComparison.Ordinal);

    internal async Task ToggleAsync(string? value)
    {
        if (value is null)
        {
            return;
        }

        if (Type == ToggleGroupType.Multiple)
        {
            var current = (Values ?? _uncontrolledValues).ToArray();
            var next = current.Contains(value, StringComparer.Ordinal)
                ? current.Where(item => !string.Equals(item, value, StringComparison.Ordinal)).ToArray()
                : current.Append(value).ToArray();

            _uncontrolledValues.Clear();
            _uncontrolledValues.AddRange(next);
            await ValuesChanged.InvokeAsync(next);
            StateHasChanged();
            return;
        }

        var nextValue = string.Equals(Value ?? _uncontrolledValue, value, StringComparison.Ordinal) ? null : value;
        _uncontrolledValue = nextValue;
        await ValueChanged.InvokeAsync(nextValue);
        StateHasChanged();
    }
}
