using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>SelectContent</c>: the listbox panel, the list viewport and the two scroll buttons.
/// </summary>
/// <remarks>
/// The panel is mounted while the select is closed (with the <c>hidden</c> attribute) so the items
/// keep registering their content for <see cref="SelectValue"/> and the native form options, the
/// same way Radix keeps them in a detached fragment. Placement is either <c>item-aligned</c> (the
/// shadcn default: the checked item sits over the trigger) or <c>popper</c>.
/// </remarks>
public partial class SelectContent : ShadcnDismissableBase
{
    private ElementReference _panel;

    /// <summary>The select root this content belongs to.</summary>
    [CascadingParameter]
    public Select? Root { get; set; }

    /// <summary>Radix <c>position</c>: <c>item-aligned</c> by default, or <c>popper</c>.</summary>
    [Parameter]
    public SelectPosition Position { get; set; } = SelectPosition.ItemAligned;

    /// <summary>Alignment against the trigger when <see cref="Position"/> is <c>popper</c>.</summary>
    [Parameter]
    public string Align { get; set; } = "center";

    /// <summary>Gap between trigger and panel for the <c>popper</c> position (4 by default).</summary>
    [Parameter]
    public double SideOffset { get; set; } = 4;

    /// <summary>The <see cref="SelectGroup"/>, <see cref="SelectLabel"/>, <see cref="SelectItem"/> and <see cref="SelectSeparator"/> children.</summary>
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
                originVar: "--radix-select-content-transform-origin",
                availableHeightVar: "--radix-select-content-available-height",
                focus: "first",
                contentId: Root.ContentId,
                listbox: true,
                itemAligned: Position == SelectPosition.ItemAligned);
        }
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) => SyncAsync(_panel, Root?.IsVisible == true);

    /// <inheritdoc />
    protected override Task RequestDismissAsync() => Root?.CloseAsync() ?? Task.CompletedTask;
}
