using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>HoverCardContent</c>: a dismissable card shown next to its trigger.</summary>
public partial class HoverCardContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The hover card root this content belongs to.</summary>
    [CascadingParameter]
    public HoverCard? Root { get; set; }

    /// <summary>Alignment against the trigger (<c>center</c> by default, like shadcn).</summary>
    [Parameter]
    public string Align { get; set; } = "center";

    /// <summary>Gap between trigger and card (4 by default).</summary>
    [Parameter]
    public double SideOffset { get; set; } = 4;

    /// <summary>Card body.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Root is not null)
        {
            ConfigurePosition(
                Root.TriggerId,
                "bottom",
                Align,
                SideOffset,
                originVar: "--radix-hover-card-content-transform-origin",
                focus: "none",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
