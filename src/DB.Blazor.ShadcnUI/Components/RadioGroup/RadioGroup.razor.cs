using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>RadioGroup</c> root: a cascading context holding the selected value, the same role
/// Radix's context plays. Items are plain buttons with <c>role="radio"</c> and <c>data-state</c>.
/// </summary>
public partial class RadioGroup : ShadcnComponent
{
    /// <summary>Selected value (<c>@bind-Value</c>).</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Selection change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Disables every item in the group.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Layout direction of the group (Tailwind classes decide the visual order).</summary>
    [Parameter]
    public ShadcnOrientation Orientation { get; set; } = ShadcnOrientation.Horizontal;

    /// <summary>The <see cref="RadioGroupItem"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Called by an item when it is activated.</summary>
    internal async Task SelectAsync(string? value)
    {
        if (Value == value)
        {
            return;
        }

        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
