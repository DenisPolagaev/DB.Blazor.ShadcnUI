using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>ContextMenuContent</c>: a menu panel placed at the pointer.</summary>
public partial class ContextMenuContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The context menu root this content belongs to.</summary>
    [CascadingParameter]
    public ContextMenu? Root { get; set; }

    /// <summary>Menu content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Root is not null)
        {
            ConfigurePosition(
                Root.TriggerId,
                originVar: "--radix-context-menu-content-transform-origin",
                focus: "first",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
