using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>Settings cascaded by <see cref="TooltipProvider"/> (Radix's provider props).</summary>
/// <param name="DelayDuration">Milliseconds before a tooltip opens. shadcn's provider sets 0.</param>
/// <param name="SkipDelayDuration">Milliseconds a tooltip stays open when moving between triggers.</param>
public sealed record TooltipSettings(int DelayDuration = 0, int SkipDelayDuration = 300);

/// <summary>
/// shadcn/ui <c>TooltipProvider</c>. Radix uses it to keep a shared open/close timer; the Blazor
/// port cascades the timings instead and lets each tooltip own its timer.
/// </summary>
public partial class TooltipProvider : ShadcnComponent
{
    /// <summary>Delay before a tooltip opens, 0 by default like shadcn.</summary>
    [Parameter]
    public int DelayDuration { get; set; }

    /// <summary>Grace period when moving between tooltips.</summary>
    [Parameter]
    public int SkipDelayDuration { get; set; } = 300;

    /// <summary>Provider content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
