using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>MenubarContent</c>: the panel of one menubar menu.</summary>
public partial class MenubarContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The menubar menu this content belongs to.</summary>
    [CascadingParameter]
    public MenubarMenu? Root { get; set; }

    /// <summary>Alignment against the trigger.</summary>
    [Parameter]
    public string Align { get; set; } = "start";

    /// <summary>Gap between trigger and panel (4 by default, like shadcn).</summary>
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
                "bottom",
                Align,
                SideOffset,
                originVar: "--radix-menubar-content-transform-origin",
                focus: "first",
                contentId: Root.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
