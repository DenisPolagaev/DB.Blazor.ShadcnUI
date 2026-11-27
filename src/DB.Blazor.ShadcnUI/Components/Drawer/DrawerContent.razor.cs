using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>DrawerContent</c>: a modal panel docked to an edge (vaul's drawer).</summary>
public partial class DrawerContent : ShadcnOverlayBase
{
    private ElementReference _panel;

    /// <summary>The drawer root this content belongs to.</summary>
    [CascadingParameter]
    public Drawer? Root { get; set; }

    /// <summary>Drawer body.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.ContentId, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;

    private Task CloseAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
