using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>shadcn/ui <c>SheetContent</c>: a modal panel docked to one edge.</summary>
public partial class SheetContent : ShadcnOverlayBase
{
    private ElementReference _panel;

    /// <summary>The sheet root this content belongs to.</summary>
    [CascadingParameter]
    public Sheet? Root { get; set; }

    /// <summary>Sheet body.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.ContentId, Root?.IsOpen == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;

    private Task CloseAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
