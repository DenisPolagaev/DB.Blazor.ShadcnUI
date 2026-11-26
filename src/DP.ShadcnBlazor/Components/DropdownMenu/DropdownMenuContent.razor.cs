using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// shadcn/ui <c>DropdownMenuContent</c>: a dismissable menu panel with roving arrow-key focus and
/// the Radix <c>--radix-dropdown-menu-content-available-height</c> variable shadcn's max-height
/// utility reads.
/// </summary>
public partial class DropdownMenuContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The dropdown root this content belongs to.</summary>
    [CascadingParameter]
    public DropdownMenu? Root { get; set; }

    /// <summary>Preferred side: <c>bottom</c>.</summary>
    [Parameter]
    public string Side { get; set; } = "bottom";

    /// <summary>Alignment: <c>center</c> by default, <c>start</c>/<c>end</c> like Radix.</summary>
    [Parameter]
    public string Align { get; set; } = "center";

    /// <summary>Gap between trigger and panel (4 by default).</summary>
    [Parameter]
    public double SideOffset { get; set; } = 4;

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
                Side,
                Align,
                SideOffset,
                originVar: "--radix-dropdown-menu-content-transform-origin",
                availableHeightVar: "--radix-dropdown-menu-content-available-height",
                focus: "first",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsOpen == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
