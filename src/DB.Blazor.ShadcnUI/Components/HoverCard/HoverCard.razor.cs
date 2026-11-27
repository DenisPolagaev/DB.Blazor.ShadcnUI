using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// Radix's <c>HoverCard</c>: a floating card shown on hover/focus with an open and a close delay.
/// </summary>
public partial class HoverCard : ShadcnFloatingRoot
{
    /// <summary>Delay before the card appears (Radix default 700 ms).</summary>
    [Parameter]
    public int OpenDelay { get; set; } = 700;

    /// <summary>Grace period before the card closes (Radix default 300 ms).</summary>
    [Parameter]
    public int CloseDelay { get; set; } = 300;
}
