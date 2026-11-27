using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>TooltipContent</c>: a non-interactive label placed above the trigger.</summary>
public partial class TooltipContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The tooltip root this content belongs to.</summary>
    [CascadingParameter]
    public Tooltip? Root { get; set; }

    /// <summary>Preferred side, <c>top</c> like Radix.</summary>
    [Parameter]
    public string Side { get; set; } = "top";

    /// <summary>Gap between trigger and panel (Radix <c>sideOffset</c>, 0 by default in shadcn).</summary>
    [Parameter]
    public double SideOffset { get; set; }

    /// <summary>Tooltip text.</summary>
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
                "center",
                SideOffset,
                originVar: "--radix-tooltip-content-transform-origin",
                focus: "none",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
