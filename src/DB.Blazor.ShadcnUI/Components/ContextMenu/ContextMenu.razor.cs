using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// Radix's <c>ContextMenu</c>: a menu opened at the pointer position on right click. The panel is
/// placed from the click coordinates (no JS), so it works inside any container.
/// </summary>
public partial class ContextMenu : ShadcnFloatingRoot
{
    private double _x;
    private double _y;

    /// <summary>Panel offset from the pointer, Radix's <c>sideOffset</c> (0 by default).</summary>
    [Parameter]
    public double Offset { get; set; }

    /// <summary>Left coordinate of the panel, in pixels.</summary>
    public double Left => _x + Offset;

    /// <summary>Top coordinate of the panel, in pixels.</summary>
    public double Top => _y + Offset;

    /// <summary>Opens the menu at the pointer position.</summary>
    public Task OpenAtAsync(double x, double y)
    {
        _x = x;
        _y = y;
        return ShowAsync();
    }
}
