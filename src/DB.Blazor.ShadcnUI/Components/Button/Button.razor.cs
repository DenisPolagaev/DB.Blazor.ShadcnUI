using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>Button</c>. Renders <c>&lt;button data-slot="button"&gt;</c> with the shadcn
/// variant/size class list.
/// </summary>
public partial class Button : ShadcnComponent
{
    /// <summary>Visual variant, <c>default</c> out of the box.</summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>Control size, <c>default</c> out of the box.</summary>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>Native <c>type</c>; <c>button</c> by default so a form is never submitted by accident.</summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>Disables the native button (<c>disabled</c> + the shadcn disabled classes).</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Button content (label and/or an icon).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Click handler.</summary>
    [Parameter]
    public EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs> OnClick { get; set; }

    /// <inheritdoc />
    protected override string SlotName => "button";

    private string? Classes => MergeClass(ButtonVariants.Get(Variant, Size));
}
