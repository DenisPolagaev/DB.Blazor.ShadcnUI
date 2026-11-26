using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>shadcn/ui <c>PopoverContent</c>: a dismissable floating panel anchored to its trigger.</summary>
public partial class PopoverContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The popover root this content belongs to.</summary>
    [CascadingParameter]
    public Popover? Root { get; set; }

    /// <summary>Preferred side: <c>bottom</c> (Radix default).</summary>
    [Parameter]
    public string Side { get; set; } = "bottom";

    /// <summary>Alignment against the trigger: <c>center</c> (Radix default).</summary>
    [Parameter]
    public string Align { get; set; } = "center";

    /// <summary>Gap between trigger and panel (Radix <c>sideOffset</c>, 4 by default).</summary>
    [Parameter]
    public double SideOffset { get; set; } = 4;

    /// <summary>Panel content.</summary>
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
                originVar: "--radix-popover-content-transform-origin",
                focus: "panel",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
