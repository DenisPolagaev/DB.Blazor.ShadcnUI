using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>shadcn/ui <c>DropdownMenuSubContent</c>: a menu nested to the right of its trigger.</summary>
public partial class DropdownMenuSubContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The submenu that owns this content.</summary>
    [CascadingParameter]
    public DropdownMenuSub? Owner { get; set; }

    /// <summary>Submenu content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Owner is not null)
        {
            ConfigurePosition(
                Owner.TriggerId,
                "right",
                "start",
                4,
                originVar: "--radix-dropdown-menu-content-transform-origin",
                focus: "none",
                contentId: Owner.ContentId);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Owner?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Owner?.CloseAsync() ?? Task.CompletedTask;

    private void CancelClose() => Owner?.CancelClose();

    private void ScheduleClose() => Owner?.ScheduleClose();
}
