using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// shadcn/ui <c>DialogContent</c>: overlay + panel. There is no DOM portal in Blazor, so the
/// component renders a <c>data-slot="dialog-portal"</c> wrapper in place and positions the panel
/// with <c>position: fixed</c>; the modal behaviour (focus trap, Escape, scroll lock) is attached
/// after the first render.
/// </summary>
public partial class DialogContent : ShadcnOverlayBase
{
    private ElementReference _panel;

    /// <summary>The dialog root this content belongs to.</summary>
    [CascadingParameter]
    public Dialog? Root { get; set; }

    /// <summary>Renders the built-in close button (shadcn's <c>showCloseButton</c>).</summary>
    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>Whether a click on the overlay closes the dialog.</summary>
    [Parameter]
    public bool CloseOnInteractOutside { get; set; } = true;

    /// <summary>Dialog body.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.ContentId, Root?.IsOpen == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => CloseAsync();

    private Task CloseAsync() => Root?.CloseAsync() ?? Task.CompletedTask;

    private Task HandleOverlayClickAsync() => CloseOnInteractOutside ? CloseAsync() : Task.CompletedTask;
}
