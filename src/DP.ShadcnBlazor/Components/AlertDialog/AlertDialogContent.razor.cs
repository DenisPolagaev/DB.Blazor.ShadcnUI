using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>Panel width of an <see cref="AlertDialogContent"/> (shadcn's <c>size</c> prop).</summary>
public enum AlertDialogContentSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Small,
}

/// <summary>
/// shadcn/ui <c>AlertDialogContent</c>. It is a modal like <see cref="DialogContent"/> but cannot be
/// dismissed by clicking the overlay (an alert dialog requires an explicit action or cancel).
/// </summary>
public partial class AlertDialogContent : ShadcnOverlayBase
{
    private ElementReference _panel;

    /// <summary>The alert dialog root this content belongs to.</summary>
    [CascadingParameter]
    public AlertDialog? Root { get; set; }

    /// <summary>Panel body.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.ContentId, Root?.IsOpen == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
