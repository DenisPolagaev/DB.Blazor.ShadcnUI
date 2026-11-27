using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// vaul's <c>Drawer</c> root: a dropdown-style modal that slides from an edge. The Blazor port
/// reuses <see cref="ShadcnModalRoot"/> for open state and exposes the direction to the content.
/// </summary>
public partial class Drawer : ShadcnModalRoot
{
    /// <summary>Edge the drawer slides from (vaul's <c>direction</c>, bottom by default).</summary>
    [Parameter]
    public DrawerDirection Direction { get; set; } = DrawerDirection.Bottom;

    /// <summary><c>data-vaul-drawer-direction</c> value for the panel.</summary>
    public string DirectionValue => Direction switch
    {
        DrawerDirection.Top => "top",
        DrawerDirection.Right => "right",
        DrawerDirection.Left => "left",
        _ => "bottom",
    };
}
