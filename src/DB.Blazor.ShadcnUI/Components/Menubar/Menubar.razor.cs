using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>Radix's <c>Menubar</c>: a horizontal bar of menus that share an open state.</summary>
public partial class Menubar : ShadcnComponent
{
    /// <summary>The bar content: <see cref="MenubarMenu"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string? Classes => MergeClass(
        "bg-background flex h-9 items-center gap-1 rounded-md border p-1 shadow-xs");
}
