using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>Radio group inside a dropdown menu; owns the selected value for its items.</summary>
public partial class DropdownMenuRadioGroup : ShadcnComponent
{
    /// <summary>Selected value (<c>@bind-Value</c>).</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Selection change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary><see cref="DropdownMenuRadioItem"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when <paramref name="value"/> is selected.</summary>
    public bool IsSelected(string? value) => string.Equals(Value, value, StringComparison.Ordinal);

    internal async Task SelectAsync(string? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
